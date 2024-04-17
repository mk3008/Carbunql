using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class BetweenClauseParser
{
    public static BetweenClause Parse(ValueBase value, string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(value, r, false);
    }

    public static BetweenClause Parse(ValueBase value, ITokenReader r, bool isNegative)
    {
        r.Read("between");

        var start = ValueParser.ParseCore(r);
        r.Read("and");
        var end = ValueParser.ParseCore(r);
        return new BetweenClause(value, start, end, isNegative);
    }
}