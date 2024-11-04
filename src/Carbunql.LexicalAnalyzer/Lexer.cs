using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    public static IEnumerable<Lex> ReadExpressionLexes(ReadOnlyMemory<char> memory, int position)
    {
        SkipWhiteSpacesAndComment(memory, ref position);

        var length = memory.Length;
        if (length < position)
        {
            yield break;
        }

        Lex lex;
        while (position < length)
        {
            // wild card
            if (TryParseWildCard(memory, ref position, out lex))
            {
                yield return lex;
                break;
            }

            // value
            if (TryParseSingleQuotedText(memory, ref position, out lex)
                || TryParseNumericValue(memory, ref position, out lex)
                || TryParseSpecialValue(memory, ref position, out lex))
            {
                yield return lex;
                SkipWhiteSpacesAndComment(memory, ref position);

                // operator
                if (TryParseOperator(memory, ref position, out lex))
                {
                    yield return lex;
                    SkipWhiteSpacesAndComment(memory, ref position);
                    continue;
                }

                // alias, expression separator, or 'from' keyword
                break;
            }

            // column
            if (TryParseSchemaOrTableOrColumn(memory, ref position, out lex))
            {
                while (lex.Type == LexType.SchemaOrTable)
                {
                    yield return lex;
                    ParseIdentifierSeparator(memory, ref position);
                    if (!TryParseSchemaOrTableOrColumn(memory, ref position, out lex))
                    {
                        throw new FormatException();
                    }
                }
                if (lex.Type != LexType.Column)
                {
                    throw new FormatException();
                }
                yield return lex;
                SkipWhiteSpacesAndComment(memory, ref position);

                // operator
                if (TryParseOperator(memory, ref position, out lex))
                {
                    yield return lex;
                    SkipWhiteSpacesAndComment(memory, ref position);
                    continue;
                }

                // alias, expression separator, or 'from' keyword
                break;
            }

            if (TryParseLeftParen(memory, ref position, out lex))
            {
                // 左カッコが出現する可能性がある
                // SELECT句が来る可能性がある
                // あとは同じ
            }
        }
    }


    public static Lex TokenizeAsQueryStart(ReadOnlyMemory<char> memory)
    {
        int position = 0;

        SkipWhiteSpaces(memory, ref position);

        // Discard all comments before the query starts
        position = ParseUntilNonComment(memory, previous: null).LastOrDefault().EndPosition;
        SkipWhiteSpaces(memory, ref position);

        if (memory.Length < position + 1)
        {
            throw new FormatException("The SQL string is empty or in an invalid format.");
        }

        // The first character must be a comment start or a reserved word; otherwise, it's an error.
        Lex lex;
        if (TryParseWithOrRecursiveLex(memory, ref position, out lex)) return lex;
        if (TryParseSelectLex(memory, ref position, out lex)) return lex;
        if (TryParseInsertLex(memory, ref position, out lex)) return lex;
        if (TryParseDeleteLex(memory, ref position, out lex)) return lex;
        if (TryParseUpdateLex(memory, ref position, out lex)) return lex;
        if (TryParseMergeLex(memory, ref position, out lex)) return lex;
        if (TryParseCreateLex(memory, ref position, out lex)) return lex;
        if (TryParseAlterLex(memory, ref position, out lex)) return lex;

        throw new FormatException("An invalid token was encountered. Please check if the SQL statement is correct.");
    }

    public static Lex TokenizeIdentifier(ReadOnlyMemory<char> memory, int position)
    {
        SkipWhiteSpaces(memory, ref position);

        if (memory.Length < position + 1)
        {
            throw new FormatException("The SQL string is empty or in an invalid format.");
        }

        // Assume some identifier can be retrieved
        // Separators like commas or dots are not expected
        Lex lex;
        if (TryParseCommentStartLex(memory, ref position, out lex)) return lex;

        if (TryParsePrefixNegationLex(memory, ref position, out lex)) return lex;
        if (TryParseLeftParen(memory, ref position, out lex)) return lex;

        if (TryParseValueLex(memory, ref position, out lex)) return lex;

        throw new FormatException("An invalid token was encountered. Please check if the SQL statement is correct.");
    }



    private static bool MemoryEqualsIgnoreCase(ReadOnlyMemory<char> memory, int position, string keyword)
    {
        if (position + keyword.Length > memory.Length)
        {
            return false;
        }

        for (int i = 0; i < keyword.Length; i++)
        {
            if (char.ToLowerInvariant(memory.Span[position + i]) != char.ToLowerInvariant(keyword[i]))
            {
                return false;
            }
        }

        return true;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseQueryTerminator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if ((memory.Span[position] == ';'))
        {
            lex = new Lex(memory, LexType.QueryTerminator, position, 1, memory.Length);
            return true;
        }

        return false;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseSingleCharLex(ReadOnlyMemory<char> memory, ref int position, char targetChar, LexType lexType, out Lex lex)
    {
        lex = default;

        if (memory.Length < position + 1)
        {
            return false;
        }

        if (memory.Span[position] == targetChar)
        {
            lex = new Lex(memory, lexType, position, 1);
            return true;
        }

        return false;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseValueSeparator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        return TryParseSingleCharLex(memory, ref position, ',', LexType.ValueSeparator, out lex);
    }

    private static Lex ParseIdentifierSeparator(ReadOnlyMemory<char> memory, ref int position)
    {
        if (memory.Length < position + 1 || memory.Span[position] != '.')
        {
            throw new Exception();
        }
        var start = position;
        position++;
        return new Lex(memory, LexType.IdentifierSeparator, start, 1);
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseIdentifierSeparator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        return TryParseSingleCharLex(memory, ref position, '.', LexType.IdentifierSeparator, out lex);
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseLeftParen(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        return TryParseSingleCharLex(memory, ref position, '(', LexType.LeftParen, out lex);
    }







    [MemberNotNullWhen(true)]
    private static bool TryParsePrefixNegationLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if (memory.Span[position] == '~')
        {
            lex = new Lex(memory, LexType.PrefixNegation, position, 1);
            return true;
        }

        if (memory.Length < position + 3)
        {
            return false;
        }

        if (memory.EqualsWordIgnoreCase(position, "not"))
        {
            lex = new Lex(memory, LexType.PrefixNegation, position, 3);
            return true;
        }

        return false;
    }















}




//private static bool IsKeyword(ReadOnlySpan<char> sql, int position, string keyword)
//{
//    // Check if the keyword matches at the given position
//    return sql.Slice(position, keyword.Length).ToString().Equals(keyword, StringComparison.OrdinalIgnoreCase);
//}


//private static LexType FindLex(ReadOnlyMemory<char> memory, char firstChar, ref int pos)
//{
//    if (char.IsDigit(firstChar))
//    {
//    }

//    return LexType.Unknown; // Default return
//}
//if (pos < length)
//{
//    char current = sql[pos];
//    LexType lexType = current switch
//    {
//        ';' => LexType.QueryTerminator,
//        ',' => LexType.ValueSeparator,
//        '.' => LexType.IdentifierSeparator,
//        '(' => LexType.LeftParen,
//        ')' => LexType.RightParen,
//        '[' => LexType.LeftSquareBracket,
//        ']' => LexType.RightSquareBracket,
//        _ => FindLex(memory, current, ref pos)
//    };

//    if (lexType == LexType.Unknown)
//    {
//        throw new InvalidOperationException($"Unexpected character '{current}' at position {pos}.");
//    }

//    return new Lex(memory, lexType, start, pos - start + 1); // Adjusted length calculation
//}


//public static IEnumerable<SqlToken> Tokenize(string sql)
//{
//    return Tokenize(sql.AsMemory(), pos: 0, length: sql.Length, parenthesisLevel: 0, bracketLevel: 0);
//}

//internal static IEnumerable<SqlToken> Tokenize(ReadOnlyMemory<char> memory, int pos, int length, int parenthesisLevel, int bracketLevel)
//{
//    var sql = memory.Span;
//    int start;

//    while (pos < length)
//    {
//        start = pos;
//        char current = sql[pos];
//        switch (current)
//        {
//            case ' ' or '\t' or '\n':
//                pos++; // skip whitespace
//                break;

//            case '.':
//                pos++;

//                yield return new SqlToken(TokenType.Dot, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, ".");
//                break;

//            case ',':
//                pos++;

//                yield return new SqlToken(TokenType.Comma, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "\'");
//                break;

//            case '(':
//                pos++;

//                yield return new SqlToken(TokenType.LeftParen, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "(");
//                parenthesisLevel++;
//                break;

//            case ')':
//                pos++;

//                parenthesisLevel--;
//                yield return new SqlToken(TokenType.RightParen, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, ")");
//                break;

//            case '[':
//                pos++;

//                yield return new SqlToken(TokenType.LeftBracket, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "[");
//                bracketLevel++;
//                break;

//            case ']':
//                pos++;

//                bracketLevel--;
//                yield return new SqlToken(TokenType.RightBracket, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel, "]");
//                break;

//            case '/':
//                if (TokenParser.TryParseHintCommentToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var startHint, out var hint, out var endHint))
//                {
//                    pos = endHint.EndPosition;
//                    yield return startHint;
//                    yield return hint;
//                    yield return endHint;
//                    break;
//                }

//                if (TokenParser.TryParseBlockCommentToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var startBlockComment, out var blockComment, out var endBlockComment))
//                {
//                    pos = endBlockComment.EndPosition;
//                    yield return startBlockComment;
//                    yield return blockComment;
//                    yield return endBlockComment;
//                    break;
//                }

//                if (TokenParser.TryParseSymbolOperatorToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var slashOperator))
//                {
//                    pos = slashOperator.EndPosition;
//                    yield return slashOperator;
//                    break;
//                }

//                throw new InvalidOperationException();

//            case '-':
//                if (TokenParser.TryParseLineCommentToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var startLineComment, out var lineComment))
//                {
//                    pos = lineComment.EndPosition;
//                    yield return startLineComment;
//                    yield return lineComment;
//                    break;
//                }

//                if (TokenParser.TryParseSymbolOperatorToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var minusOperator))
//                {
//                    pos = minusOperator.EndPosition;
//                    yield return minusOperator;
//                    break;
//                }

//                throw new InvalidOperationException();

//            case '\'':
//                if (TokenParser.TryParseSingleQuoteIdentifierToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var singleQuotedIdentifer))
//                {
//                    pos = singleQuotedIdentifer.EndPosition;
//                    yield return singleQuotedIdentifer;
//                }

//                throw new InvalidOperationException();

//            case '"':
//                if (TokenParser.TryParseDoubleQuoteIdentifierToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var doubleQuotedIdentifer))
//                {
//                    pos = doubleQuotedIdentifer.EndPosition;
//                    yield return doubleQuotedIdentifer;
//                }

//                throw new InvalidOperationException();

//            default:
//                if (char.IsDigit(current))
//                {
//                    if (TokenParser.TryParseNumericToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var numericToken))
//                    {
//                        pos = numericToken.EndPosition;
//                        yield return numericToken;
//                    }
//                }
//                else if (char.IsLetter(current))
//                {
//                    var identifier = TokenParser.ReadLowercaseLetterIdentifier(memory, pos, length);

//                    if (TokenParser.TryParseSpecialValueToken(memory, ref pos, length, parenthesisLevel, ref bracketLevel, identifier, out var specialValueToken))
//                    {
//                        pos = specialValueToken.EndPosition;
//                        yield return specialValueToken;
//                        break;

//                    }

//                    if (TokenParser.TryParseTextOperatorToken(memory, pos, length, parenthesisLevel, ref bracketLevel, identifier, out var logicalToken))
//                    {
//                        pos = logicalToken.EndPosition;
//                        yield return logicalToken;
//                        break;
//                    }

//                    // type

//                    if (TokenParser.TryParseIntervalTypeToken(memory, ref pos, length, parenthesisLevel, ref bracketLevel, identifier, out var intervalToken))
//                    {
//                        pos = intervalToken.EndPosition;
//                        yield return intervalToken;
//                        break;
//                    }

//                    if (TokenParser.TryParseDateTimeTypeToken(memory, pos, length, parenthesisLevel, bracketLevel, identifier, out var datetimeTypeToken))
//                    {
//                        pos = datetimeTypeToken.EndPosition;
//                        yield return datetimeTypeToken;
//                        break;
//                    }


//                    // ddl command

//                    if (TokenParser.TryParseCreateIdentifierToken(memory, ref pos, length, parenthesisLevel, ref bracketLevel, identifier, out var createToken))
//                    {
//                        pos = createToken.EndPosition;
//                        yield return createToken;
//                        break;
//                    }

//                    if (TokenParser.TryParseAlterIdentifierToken(memory, ref pos, length, parenthesisLevel, ref bracketLevel, identifier, out var alterToken))
//                    {
//                        pos = alterToken.EndPosition;
//                        yield return alterToken;
//                        break;
//                    }

//                    if (TokenParser.TryParseDropIdentifierToken(memory, ref pos, length, parenthesisLevel, ref bracketLevel, identifier, out var dropToken))
//                    {
//                        pos = dropToken.EndPosition;
//                        yield return dropToken;
//                        break;
//                    }
//                }
//                else
//                {
//                    // not letter and not digit
//                    if (TokenParser.TryParseSymbolOperatorToken(memory, pos, length, parenthesisLevel, ref bracketLevel, out var defaultOperator))
//                    {
//                        pos = defaultOperator.EndPosition;
//                        yield return defaultOperator;
//                        break;
//                    }
//                }

//                yield return new SqlToken(TokenType.Identifier, memory.Slice(start, pos - start), start, pos, parenthesisLevel, bracketLevel);
//                break;
//        }
//    }
//}



