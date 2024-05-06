using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a GROUP BY clause from SQL text or token streams.
/// </summary>
public class GroupClauseParser
{
    /// <summary>
    /// Parses a GROUP BY clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the GROUP BY clause.</param>
    /// <returns>The parsed GROUP BY clause.</returns>
    public static GroupClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a GROUP BY clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed GROUP BY clause.</returns>
    public static GroupClause Parse(ITokenReader r)
    {
        var vals = ValueCollectionParser.Parse(r);
        var group = new GroupClause(vals);
        return group;
    }
}
