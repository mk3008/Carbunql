using Carbunql.Core.Clauses;

namespace Carbunql.Analysis.Parser;

public class WhereClauseParser
{
    public static WhereClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static WhereClause Parse(TokenReader r)
    {
        var val = ValueParser.Parse(r);
        var where = new WhereClause(val);
        return where;
    }
}