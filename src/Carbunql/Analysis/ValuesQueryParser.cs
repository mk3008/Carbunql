using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Static class for parsing VALUES queries.
/// </summary>
public static class ValuesQueryParser
{
    /// <summary>
    /// Parses a VALUES query from the specified text.
    /// </summary>
    /// <param name="text">The text containing the VALUES query.</param>
    /// <returns>The parsed VALUES query.</returns>
    public static ValuesQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
        }

        return q;
    }

    /// <summary>
    /// Parses a VALUES query from the specified token reader.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed VALUES query.</returns>
    internal static ValuesQuery Parse(ITokenReader r)
    {
        r.Read("values");

        var sq = ValuesClauseParser.Parse(r);

        var tokens = new string[] { "union", "union all", "except", "minus", "intersect" };
        if (r.Peek().IsEqualNoCase(tokens))
        {
            var op = r.Read();
            sq.AddOperatableValue(op, Parse(r));
        }

        sq.OrderClause = ParseOrderOrDefault(r);
        sq.LimitClause = ParseLimitOrDefault(r);
        return sq;
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
