using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses RETURNING clauses from SQL text or token streams.
/// </summary>
public static class ReturningClauseParser
{
    /// <summary>
    /// Parses a RETURNING clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the RETURNING clause.</param>
    /// <returns>The parsed RETURNING clause.</returns>
    public static ReturningClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a RETURNING clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed RETURNING clause.</returns>
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
