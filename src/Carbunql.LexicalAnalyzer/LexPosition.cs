namespace Carbunql.LexicalAnalyzer;

public readonly struct LexPosition(ReadOnlyMemory<char> memory, int position, int length)
{
    public ReadOnlyMemory<char> Memory { get; } = memory;

    public int Position { get; } = position;

    public int Length { get; } = length;

    public int EndPosition => Position + Length;

    public string Value => GetValue();

    private readonly string? cachedValue;

    private string GetValue()
    {
        if (cachedValue == null)
        {
            if (Position + Length > Memory.Length)
                throw new ArgumentOutOfRangeException(nameof(Length), "Position and Length exceed memory bounds.");

            return Memory.Slice(Position, Length).ToString();
        }
        return cachedValue;
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



