using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class SelectQueryParser
{
    internal static SelectQuery Parse(string text, WithClause? w = null)
    {
        using var r = new TokenReader(text);
        var sq = Parse(r);
        if (w != null) sq.WithClause = w;
        return sq;
    }

    internal static SelectQuery Parse(TokenReader r, WithClause? w)
    {
        var sq = Parse(r);
        if (w != null) sq.WithClause = w;
        return sq;
    }

    internal static SelectQuery Parse(TokenReader r)
    {
        var sq = new SelectQuery();

        r.ReadToken("select");

        sq.SelectClause = SelectClauseParser.Parse(r);
        sq.FromClause = ParseFromOrDefault(r);
        sq.WhereClause = ParseWhereOrDefault(r);
        sq.GroupClause = ParseGroupOrDefault(r);
        sq.HavingClause = ParseHavingOrDefault(r);
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

    private static FromClause? ParseFromOrDefault(TokenReader r)
    {
        if (r.TryReadToken("from") == null) return null;
        return FromClauseParser.Parse(r);
    }

    private static WhereClause? ParseWhereOrDefault(TokenReader r)
    {
        if (r.TryReadToken("where") == null) return null;
        return WhereClauseParser.Parse(r);
    }

    private static GroupClause? ParseGroupOrDefault(TokenReader r)
    {
        if (r.TryReadToken("group") == null) return null;
        return GroupClauseParser.Parse(r);
    }

    private static HavingClause? ParseHavingOrDefault(TokenReader r)
    {
        if (r.TryReadToken("having") == null) return null;
        return HavingClauseParser.Parse(r);
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