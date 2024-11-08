using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    /// <summary>
    /// Defines a set of characters considered as symbols that terminate an identifier.
    /// </summary>
    private static readonly HashSet<char> Symbols = new HashSet<char>
    {
        '+', '-', '*', '/', '%',    // Arithmetic operators
        '(', ')', '[', ']', '{', '}',  // Brackets and braces
        '~', '@', '#', '$', '^', '&',  // Special symbols
        '!', '?', ':', ';', ',', '.', '<', '>', '=', '|', '\\', // Other symbols
        '`', '"', '\''                 // Quotation marks
    };

    /// <summary>
    /// Attempts to find the end position of a valid identifier starting at a given position in the specified memory segment.
    /// </summary>
    /// <param name="memory">The memory segment containing the characters to analyze.</param>
    /// <param name="startPosition">The initial position to start searching for the identifier.</param>
    /// <param name="position">
    /// The position marking the end of the identifier if found, or the start position if the search fails.
    /// </param>
    /// <returns>
    /// <c>true</c> if a valid identifier was found; <c>false</c> if the identifier is invalid (e.g., starts with a digit).
    /// </returns>
    public static bool TryGetCharacterEndPosition(ReadOnlyMemory<char> memory, in int startPosition, out int position)
    {
        position = startPosition;

        memory.SkipWhiteSpacesAndComment(ref position);

        // Disallow identifiers that start with a digit.
        if (char.IsDigit(memory.Span[position]))
        {
            return false;
        }

        // Loop until the end of the identifier or a symbol/whitespace is encountered.
        while (!memory.IsAtEnd(position))
        {
            var current = memory.Span[position];
            if (char.IsWhiteSpace(current) || Symbols.Contains(current))
            {
                break;
            }
            position++;
        }

        // Return false if no valid identifier was found.
        return (position != startPosition);
    }



    public static IEnumerable<Lex> ReadExpressionLexes(ReadOnlyMemory<char> memory, int position)
    {
        memory.SkipWhiteSpacesAndComment(ref position);

        if (memory.IsAtEnd(position))
        {
            yield break;
        }

        Lex lex;
        while (!memory.IsAtEnd(position))
        {
            // wildcard
            if (memory.TryParseWildCard(ref position, out lex))
            {
                yield return lex;
                break;
            }

            // paren
            if (TryParseLeftParen(memory, ref position, out lex))
            {
                yield return lex;
                memory.SkipWhiteSpacesAndComment(ref position);

                // 次のトークンがselectの場合、インラインクエリのため特殊

                // それ以外は再帰処理
                foreach (var innerLex in ReadExpressionLexes(memory, position))
                {
                    yield return innerLex;
                    position = innerLex.EndPosition;
                }

                // close paren
                lex = ParseRightParen(memory, ref position);
                yield return lex;

                // operator check
                memory.SkipWhiteSpacesAndComment(ref position);
                if (TryParseOperator(memory, ref position, out lex))
                {
                    yield return lex;
                    memory.SkipWhiteSpacesAndComment(ref position);
                    continue;
                }

                // alias, expression separator, or 'from' keyword
                break;
            }

            // value
            if (TryParseSingleQuotedText(memory, ref position, out lex)
                || TryParseNumericValue(memory, ref position, out lex))
            //|| TryParseSpecialValue(memory, ref position, out lex))
            {
                yield return lex;

                // operator check
                memory.SkipWhiteSpacesAndComment(ref position);
                if (TryParseOperator(memory, ref position, out lex))
                {
                    yield return lex;
                    memory.SkipWhiteSpacesAndComment(ref position);
                    continue;
                }

                // alias, expression separator, or 'from' keyword
                break;
            }

            // 一旦読み込む
            var value = ReadCharactorIdentifier(memory, ref position);

            //identifierが特殊文字と一致するなら特殊文字
            //次がカッコなら関数
            //次が.ならnamespace
            //それ以外は列

            // function
            if (TryParseFunction(memory, ref position, out lex))
            {

            }

            // column
            if (TryParseNamespaceOrColumn(memory, ref position, out lex))
            {
                while (lex.Type == LexType.Namespace)
                {
                    yield return lex;

                    lex = ParseIdentifierSeparator(memory, ref position);
                    yield return lex;

                    if (!TryParseNamespaceOrColumn(memory, ref position, out lex))
                    {
                        throw new FormatException();
                    }
                }
                if (lex.Type != LexType.Column)
                {
                    throw new FormatException();
                }
                yield return lex;

                // operator check
                memory.SkipWhiteSpacesAndComment(ref position);
                if (TryParseOperator(memory, ref position, out lex))
                {
                    yield return lex;
                    memory.SkipWhiteSpacesAndComment(ref position);
                    continue;
                }

                // alias, expression separator, or 'from' keyword
                break;
            }


        }
    }

    [MemberNotNullWhen(true)]
    public static bool TryParseExpressionName(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if (memory.IsAtEnd(position))
        {
            return false;
        }

        memory.SkipWhiteSpacesAndComment(ref position);
        SkipAliasCommand(memory, ref position);
        memory.SkipWhiteSpacesAndComment(ref position);

        if (TryParseNamespaceOrColumn(memory, ref position, out lex))
        {
            if (lex.Type == LexType.Column)
            {
                return true;
            }
            throw new FormatException();
        }

        return false;
    }
}
