namespace Carbunql.LexicalAnalyzer;

public static class SelectQueryParser
{
    public static object Parse(string sql)
    {
        var memory = sql.AsMemory();
        var position = 0;

        Lexer.SkipWhiteSpaces(memory, ref position);

        Lexer.SkipComment(memory, ref position);

        Lexer.SkipWhiteSpaces(memory, ref position);

        Lex lex;
        if (Lexer.TryParseWithOrRecursiveLex(memory, ref position, out lex))
        {
            throw new NotImplementedException();
        }
        else if (Lexer.TryParseSelectLex(memory, ref position, out lex))
        {
            // value

        }

        throw new FormatException();
    }

    private static object ParseSelectClause(ReadOnlyMemory<char> memory, ref int position)
    {
        Lex lex;

        var lexes =


        if (Lexer.TryParseValueLex(memory, ref position, out lex))
        {


        }
    }
}
