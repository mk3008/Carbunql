using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses WHERE clauses from SQL text or token stream.
/// </summary>
public class WhereClauseParser
{
    /// <summary>
    /// Parses a WHERE clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text to parse.</param>
    /// <returns>The parsed WHERE clause.</returns>
    public static WhereClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a WHERE clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed WHERE clause.</returns>
    public static WhereClause Parse(ITokenReader r)
    {
        r.ReadOrDefault("where");

        var val = ValueParser.Parse(r);
        var where = new WhereClause(val);
        return where;
    }
}
