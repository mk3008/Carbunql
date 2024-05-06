using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a BETWEEN clause from SQL text or token streams.
/// </summary>
public static class BetweenClauseParser
{
    /// <summary>
    /// Parses a BETWEEN clause from SQL text.
    /// </summary>
    /// <param name="value">The value to be associated with the BETWEEN clause.</param>
    /// <param name="argument">The SQL text containing the BETWEEN clause.</param>
    /// <returns>The parsed BETWEEN clause.</returns>
    public static BetweenClause Parse(ValueBase value, string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(value, r, false);
    }

    /// <summary>
    /// Parses a BETWEEN clause from the token stream.
    /// </summary>
    /// <param name="value">The value to be associated with the BETWEEN clause.</param>
    /// <param name="r">The token reader.</param>
    /// <param name="isNegative">Indicates whether the BETWEEN clause is negative.</param>
    /// <returns>The parsed BETWEEN clause.</returns>
    public static BetweenClause Parse(ValueBase value, ITokenReader r, bool isNegative)
    {
        r.Read("between");

        var start = ValueParser.ParseCore(r);
        r.Read("and");
        var end = ValueParser.ParseCore(r);
        return new BetweenClause(value, start, end, isNegative);
    }
}
