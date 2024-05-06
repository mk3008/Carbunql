using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses SELECT clauses from SQL text or token streams.
/// </summary>
public static class SelectClauseParser
{
    /// <summary>
    /// Parses a SELECT clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the SELECT clause.</param>
    /// <returns>The parsed SELECT clause.</returns>
    public static SelectClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a SELECT clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed SELECT clause.</returns>
    public static SelectClause Parse(ITokenReader r)
    {
        r.ReadOrDefault("select");

        DistinctClause? distinct = null;
        TopClause? top = null;
        if (r.Peek().IsEqualNoCase("distinct"))
        {
            distinct = DistinctClauseParser.Parse(r);
        }
        if (r.Peek().IsEqualNoCase("top"))
        {
            top = TopParser.Parse(r);
        }
        return new SelectClause(ParseItems(r).ToList())
        {
            Distinct = distinct,
            Top = top
        };
    }

    /// <summary>
    /// Parses the selectable items in the SELECT clause.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>An enumerable collection of selectable items.</returns>
    private static IEnumerable<SelectableItem> ParseItems(ITokenReader r)
    {
        do
        {
            if (r.Peek().IsEqualNoCase(",")) r.Read();
            yield return SelectableItemParser.Parse(r);
        }
        while (r.Peek().IsEqualNoCase(","));
    }
}
