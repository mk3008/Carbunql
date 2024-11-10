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

            if (TryParseNumericValue(memory, ref position, out lex)
                || TryParseSingleQuotedValue(memory, ref position, out lex))
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

            if (TryParseCharactorValues(memory, position, out var lexes))
            {
                foreach (var item in lexes)
                {
                    position = item.EndPosition;
                    yield return item;

                    if (item.Type == LexType.Function)
                    {
                        // open paren
                        yield return ParseLeftParen(memory, ref position);

                        // read arguments
                        foreach (var argument in ReadExpressionLexes(memory, position))
                        {
                            yield return argument;
                            position = argument.EndPosition;
                        }

                        // close paren
                        yield return ParseRightParen(memory, ref position);
                    }
                }




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
        memory.EqualsWordIgnoreCase(position, "as", out position);
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
