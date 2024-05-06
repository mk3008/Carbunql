using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a DISTINCT clause from SQL text or token streams.
/// </summary>
public class DistinctClauseParser
{
    /// <summary>
    /// Parses a DISTINCT clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the DISTINCT clause.</param>
    /// <returns>The parsed DISTINCT clause.</returns>
    public static DistinctClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a DISTINCT clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed DISTINCT clause.</returns>
    public static DistinctClause Parse(ITokenReader r)
    {
        r.Read("distinct");

        if (!r.Peek().IsEqualNoCase("on"))
        {
            return new DistinctClause();
        }

        r.Read("on");
        r.Read("(");
        var values = ValueCollectionParser.Parse(r);
        r.Read(")");

        return new DistinctClause(values);
    }
}
