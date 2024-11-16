using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{




    public static IEnumerable<Lex> ReadExpressionLexes(ReadOnlyMemory<char> memory, int position, Lex? owner = null)
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

            // type
            if (TryParseDbType(memory, position, out lex, out position))
            {
                yield return lex;
                break;
            }

            // charactor value 
            if (TryParseCharactorValue(memory, position, out lex, out position))
            {
                yield return lex;

                if (lex.Type == LexType.Function)
                {
                    // open paren
                    memory.SkipWhiteSpacesAndComment(ref position);
                    yield return ParseLeftParen(memory, ref position);

                    // read arguments
                    foreach (var argument in ReadExpressionLexes(memory, position, owner = lex))
                    {
                        yield return argument;
                        position = argument.EndPosition;
                    }

                    memory.SkipWhiteSpacesAndComment(ref position);

                    if (lex.Value.ToLowerInvariant() == "cast")
                    {
                        if (memory.TryParseKeywordIgnoreCase(ref position, "as", LexType.Operator, out lex))
                        {
                            yield return lex;
                        }
                        else
                        {
                            throw new FormatException();
                        }

                        memory.SkipWhiteSpacesAndComment(ref position);

                        if (TryParseDbType(memory, position, out lex, out position))
                        {
                            yield return lex;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                    }

                    // close paren
                    memory.SkipWhiteSpacesAndComment(ref position);
                    yield return ParseRightParen(memory, ref position);
                }
                else if (lex.Type == LexType.Namespace)
                {
                    do
                    {
                        memory.SkipWhiteSpacesAndComment(ref position);
                        if (memory.Span[position] != '.')
                        {
                            throw new FormatException();
                        }
                        position++;
                        memory.SkipWhiteSpacesAndComment(ref position);

                        if (memory.TryParseWildCard(ref position, out lex))
                        {
                            yield return lex;
                            break;
                        }
                        if (!TryParseCharactorValue(memory, position, out lex, out position))
                        {
                            throw new FormatException();
                        }
                        if (!(lex.Type == LexType.Namespace || lex.Type == LexType.Column))
                        {
                            throw new FormatException();
                        }
                        yield return lex;
                    } while (lex.Type != LexType.Column);
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


            throw new FormatException();
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

        var start = position;
        memory.EqualsWordIgnoreCase(position, "as", out position);
        if (start != position)
        {
            // "as" がある場合、必ずエイリアス名が取得できる

            memory.SkipWhiteSpacesAndComment(ref position);
            start = position;
            if (!TryGetCharacterEndPosition(memory, position, out position))
            {
                throw new FormatException();
            }
            lex = new Lex(memory, LexType.Alias, start, position - start);
            return true;
        }
        else
        {
            // "as" が省略されている場合、エイリアス名は無い可能性がある
            if (!TryGetCharacterEndPosition(memory, position, out var p))
            {
                return false;
            }
            var name = memory.Slice(start, p - start).ToString();
            if (name.ToLowerInvariant() == "from")
            {
                return false;
            }
            position = p;
            lex = new Lex(memory, LexType.Alias, start, position - start, name);
            return true;
        }
    }
}
