using Carbunql.Analysis.Parser;
using Carbunql.Core;
using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis;

public static class ValuesQueryParser
{
    public static ValuesQuery Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static ValuesQuery Parse(TokenReader r)
    {
        var sq = new ValuesQuery();

        if (r.TryReadToken("with") != null)
        {
            sq.WithClause = WithClauseParser.Parse(r);
        }

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