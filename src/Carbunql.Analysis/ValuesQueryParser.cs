using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class ValuesQueryParser
{
    internal static ValuesQuery Parse(string text, WithClause? w = null)
    {
        using var r = new TokenReader(text);
        var sq = Parse(r);
        if (w != null) sq.WithClause = w;
        return sq;
    }

    internal static ValuesQuery Parse(TokenReader r, WithClause? w)
    {
        var sq = Parse(r);
        if (w != null) sq.WithClause = w;
        return sq;
    }

    internal static ValuesQuery Parse(TokenReader r)
    {
        var sq = new ValuesQuery();

        r.ReadToken("values");

        sq.ValuesClause = ValuesClauseParser.Parse(r);
        sq.OrderClause = ParseOrderOrDefault(r);

        var tokens = new string[] { "union", "except", "minus", "intersect" };
        if (r.PeekRawToken().AreContains(tokens))
        {
            var op = r.ReadToken();
            sq.AddOperatableValue(op, Parse(r));
        }

        sq.LimitClause = ParseLimitOrDefault(r);
        return sq;
    }

    private static OrderClause? ParseOrderOrDefault(TokenReader r)
    {
        if (r.TryReadToken("order") == null) return null;
        return OrderClauseParser.Parse(r);
    }

    private static LimitClause? ParseLimitOrDefault(TokenReader r)
    {
        if (r.TryReadToken("limit") == null) return null;
        return LimitClauseParser.Parse(r);
    }
}