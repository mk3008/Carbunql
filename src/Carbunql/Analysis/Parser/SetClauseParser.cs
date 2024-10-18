using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class SetClauseParser
{

    public static SetClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static SetClause Parse(ITokenReader r)
    {
        r.Read("set");

        var s = new SetClause
        {
            ValueParser.Parse(r)
        };

        while (r.Peek() == ",")
        {
            r.Read(",");
            s.Add(ValueParser.Parse(r));
        }

        return s;
    }
}
