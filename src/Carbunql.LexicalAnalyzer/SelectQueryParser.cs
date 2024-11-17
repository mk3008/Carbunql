namespace Carbunql.LexicalAnalyzer;

public static class SelectQueryParser
{
    // debug
    public static IEnumerable<Lex> Parse(string sql)
    {
        var memory = sql.AsMemory();
        var pos = 0;

        Lex lex;

        memory.SkipWhiteSpacesAndComment(pos, out pos);

        // "with"
        if (Lexer.TryParseWithOrRecursive(memory, pos, out lex, out pos))
        {
            throw new FormatException();
            memory.SkipWhiteSpacesAndComment(pos, out pos);
        }

        if (!Lexer.TryParseSelect(memory, pos, out lex, out pos))
        {
            throw new FormatException("The select query must start with either a WITH clause or a SELECT statement.");
        }

        // "select"
        yield return lex;
        memory.SkipWhiteSpacesAndComment(pos, out pos);

        if (lex.Type == LexType.SelectDistinctOn)
        {
            // paren argument
            throw new FormatException();
        }


        // select expressions
        bool hasExpression = false;
        while (true)
        {
            // expression
            foreach (var item in Lexer.ReadExpressionLexes(memory, pos))
            {
                hasExpression = true;
                pos = item.EndPosition;
                yield return item;
                memory.SkipWhiteSpacesAndComment(pos, out pos);
            }

            // alias name
            if (Lexer.TryParseExpressionName(memory, pos, out lex, out pos))
            {
                yield return lex;
                memory.SkipWhiteSpacesAndComment(pos, out pos);
            }

            //expression Separator
            if (Lexer.TryParseExpressionSeparator(memory, pos, out lex, out pos))
            {
                memory.SkipWhiteSpacesAndComment(pos, out pos);
                continue;
            }
            break;

        }

        if (hasExpression == false)
        {
            throw new FormatException();
        }

        memory.SkipWhiteSpacesAndComment(pos, out pos);

        // "from"
        lex = Lexer.ParseFrom(memory, pos, out pos);

        // table
    }
}
