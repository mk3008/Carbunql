using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a HAVING clause from SQL text or token streams.
/// </summary>
public static class HavingClauseParser
{
    /// <summary>
    /// Parses a HAVING clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the HAVING clause.</param>
    /// <returns>The parsed HAVING clause.</returns>
    public static HavingClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a HAVING clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed HAVING clause.</returns>
    public static HavingClause Parse(ITokenReader r)
    {
        var val = ValueParser.Parse(r);
        var having = new HavingClause(val);
        return having;
    }
}
