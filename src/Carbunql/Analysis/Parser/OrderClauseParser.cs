using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses ORDER BY clauses from SQL text or token streams.
/// </summary>
public static class OrderClauseParser
{
    /// <summary>
    /// Parses an ORDER BY clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the ORDER BY clause.</param>
    /// <returns>The parsed ORDER BY clause.</returns>
    public static OrderClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return new OrderClause(ReadItems(r).ToList());
    }

    /// <summary>
    /// Parses an ORDER BY clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed ORDER BY clause.</returns>
    public static OrderClause Parse(ITokenReader r)
    {
        return new OrderClause(ReadItems(r).ToList());
    }

    private static IEnumerable<IQueryCommandable> ReadItems(ITokenReader r)
    {
        do
        {
            r.ReadOrDefault(",");
            yield return SortableItemParser.Parse(r);
        }
        while (r.Peek().IsEqualNoCase(","));
    }
}
