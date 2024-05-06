using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses PARTITION BY clauses from SQL text or token streams.
/// </summary>
public static class PartitionClauseParser
{
    /// <summary>
    /// Parses a PARTITION BY clause from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the PARTITION BY clause.</param>
    /// <returns>The parsed PARTITION BY clause.</returns>
    public static PartitionClause Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return new PartitionClause(ValueCollectionParser.ReadValues(r).ToList());
    }

    /// <summary>
    /// Parses a PARTITION BY clause from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed PARTITION BY clause.</returns>
    public static PartitionClause Parse(ITokenReader r)
    {
        return new PartitionClause(ValueCollectionParser.ReadValues(r).ToList());
    }
}
