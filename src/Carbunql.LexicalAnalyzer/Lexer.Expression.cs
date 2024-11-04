using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
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

            // value
            if (TryParseSingleQuotedText(memory, ref position, out lex)
                || TryParseNumericValue(memory, ref position, out lex)
                || TryParseSpecialValue(memory, ref position, out lex))
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

            // column
            if (TryParseSchemaOrTableOrColumn(memory, ref position, out lex))
            {
                while (lex.Type == LexType.SchemaOrTable)
                {
                    yield return lex;

                    lex = ParseIdentifierSeparator(memory, ref position);
                    yield return lex;

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

        if (TryParseSchemaOrTableOrColumn(memory, ref position, out lex))
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
