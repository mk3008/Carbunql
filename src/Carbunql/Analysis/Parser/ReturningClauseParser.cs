using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class ReturningClauseParser
{
    public static ReturningClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    public static ReturningClause Parse(ITokenReader r)
    {
        r.ReadOrDefault("returning");
        return new ReturningClause(ParseItems(r).ToList());
    }

    private static IEnumerable<ValueBase> ParseItems(ITokenReader r)
    {
        do
        {
            if (r.Peek().IsEqualNoCase(",")) r.Read();
            yield return ValueParser.Parse(r);
        }
        while (r.Peek().IsEqualNoCase(","));
    }
}