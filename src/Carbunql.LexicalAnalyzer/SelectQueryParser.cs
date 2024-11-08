namespace Carbunql.LexicalAnalyzer;

public static class SelectQueryParser
{
    // debug
    public static IEnumerable<Lex> Parse(string sql)
    {
        var memory = sql.AsMemory();
        var position = 0;

        Lex lex;

        // "with"
        if (Lexer.TryParseWithOrRecursive(memory, ref position, out lex))
        {
            throw new FormatException();
        }

        if (!Lexer.TryParseSelect(memory, ref position, out lex))
        {
            throw new FormatException();
        }

        // "select"
        yield return lex;

        if (lex.Type == LexType.SelectDistinctOn)
        {
            // paren argument
            throw new FormatException();
        }

        bool hasExpression = false;
        while (true)
        {
            // expression
            foreach (var item in Lexer.ReadExpressionLexes(memory, position))
            {
                hasExpression = true;
                position = item.EndPosition;
                yield return item;
            }
            // alias name
            if (Lexer.TryParseExpressionName(memory, ref position, out lex))
            {
                yield return lex;
            }

            //expression Separator
            if (!Lexer.TryParseExpressionSeparator(memory, ref position, out lex))
            {
                break;
            }
        }

        if (hasExpression == false)
        {
            throw new FormatException();
        }

        // "from"
        lex = Lexer.ParseFrom(memory, ref position);

        // table
    }
}
