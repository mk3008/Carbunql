using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a filter from SQL text or token streams.
/// </summary>
public static class FilterParser
{
    /// <summary>
    /// Parses a filter from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the filter.</param>
    /// <returns>The parsed filter.</returns>
    public static Filter Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a filter from the token stream, treating it as an inner filter.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed filter.</returns>
    public static Filter ParseAsInner(ITokenReader r)
    {
        r.Read("(");
        using var ir = new BracketInnerTokenReader(r);
        var v = Parse(ir);
        return v;
    }

    /// <summary>
    /// Parses a filter from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed filter.</returns>
    public static Filter Parse(ITokenReader r)
    {
        r.ReadOrDefault("(");
        r.Read("where");

        var filter = new Filter() { WhereClause = WhereClauseParser.Parse(r) };
        r.ReadOrDefault(")");
        return filter;
    }
}
