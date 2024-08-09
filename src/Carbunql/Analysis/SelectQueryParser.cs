using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Provides functionality to parse SELECT queries in SQL.
/// </summary>
public static class SelectQueryParser
{
    /// <summary>
    /// Parses the specified SQL query string as a SELECT query.
    /// </summary>
    /// <param name="text">The SQL query string.</param>
    /// <returns>The parsed SelectQuery object.</returns>
    public static SelectQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);

        if (r.Peek().IsEqualNoCase("with")) return CTEQueryParser.Parse(r);
        var sq = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
        }

        return sq;
    }

    public static SelectQuery ParseAsInner(ITokenReader r)
    {
        using var ir = new BracketInnerTokenReader(r);
        var v = Parse(ir);
        return v;
    }

    /// <summary>
    /// Parses the SELECT query using the provided ITokenReader.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed SelectQuery object.</returns>
    internal static SelectQuery Parse(ITokenReader r)
    {
        var sq = ParseMain(r);

        var tokens = new string[] { "union", "union all", "except", "minus", "intersect" };
        while (r.Peek().IsEqualNoCase(tokens))
        {
            //read operator
            var op = r.Read();
            sq.AddOperatableValue(op, ReadQueryParser.ParseIgnoringBrackets(r));
        }

        sq.LimitClause = ParseLimitOrDefault(r);

        return sq;
    }

    /// <summary>
    /// Parses the main part of the SELECT query using the provided ITokenReader.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed SelectQuery object.</returns>
    private static SelectQuery ParseMain(ITokenReader r)
    {
        var sq = new SelectQuery();
        r.Read("select");

        sq.SelectClause = SelectClauseParser.Parse(r);
        sq.FromClause = ParseFromOrDefault(r);
        sq.WhereClause = ParseWhereOrDefault(r);
        sq.GroupClause = ParseGroupOrDefault(r);
        sq.HavingClause = ParseHavingOrDefault(r);
        sq.WindowClause = ParseWindowOrDefault(r);
        sq.OrderClause = ParseOrderOrDefault(r);

        return sq;
    }

    private static FromClause? ParseFromOrDefault(ITokenReader r)
    {
        if (r.ReadOrDefault("from") == null) return null;
        return FromClauseParser.Parse(r);
    }

    private static WhereClause? ParseWhereOrDefault(ITokenReader r)
    {
        if (r.ReadOrDefault("where") == null) return null;
        return WhereClauseParser.Parse(r);
    }

    private static GroupClause? ParseGroupOrDefault(ITokenReader r)
    {
        if (r.ReadOrDefault("group by") == null) return null;
        return GroupClauseParser.Parse(r);
    }

    private static HavingClause? ParseHavingOrDefault(ITokenReader r)
    {
        if (r.ReadOrDefault("having") == null) return null;
        return HavingClauseParser.Parse(r);
    }

    private static WindowClause? ParseWindowOrDefault(ITokenReader r)
    {
        if (r.ReadOrDefault("window") == null) return null;
        return WindowClauseParser.Parse(r);
    }

    private static OrderClause? ParseOrderOrDefault(ITokenReader r)
    {
        if (r.ReadOrDefault("order by") == null) return null;
        return OrderClauseParser.Parse(r);
    }

    private static LimitClause? ParseLimitOrDefault(ITokenReader r)
    {
        if (r.ReadOrDefault("limit") == null) return null;
        return LimitClauseParser.Parse(r);
    }
}
