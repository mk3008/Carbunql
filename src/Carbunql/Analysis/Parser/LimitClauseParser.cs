using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a LIMIT clause from SQL text or token streams.
/// </summary>
public static class LimitClauseParser
{
    /// <summary>
    /// Parses a LIMIT clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the LIMIT clause.</param>
    /// <returns>The parsed LIMIT clause.</returns>
    public static LimitClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a LIMIT clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed LIMIT clause.</returns>
    public static LimitClause Parse(ITokenReader r)
    {
        var condition = ParseItems(r).ToList();
        if (r.ReadOrDefault("offset") != null)
        {
            var offset = ValueParser.Parse(r);
            return new LimitClause(condition) { Offset = offset };
        }
        return new LimitClause(condition);
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
