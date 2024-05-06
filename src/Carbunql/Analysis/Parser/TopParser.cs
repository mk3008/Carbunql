using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses the TOP clause from SQL text or token streams.
/// </summary>
public static class TopParser
{
    /// <summary>
    /// Parses the TOP clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the TOP clause.</param>
    /// <returns>The parsed TOP clause.</returns>
    public static TopClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses the TOP clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed TOP clause.</returns>
    public static TopClause Parse(ITokenReader r)
    {
        r.Read("Top");
        var value = ValueParser.Parse(r);
        return new TopClause(value);
    }
}
