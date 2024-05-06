using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a LIKE clause from SQL text or token streams.
/// </summary>
public static class LikeClauseParser
{
    /// <summary>
    /// Parses a LIKE clause from SQL text.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="argument">The SQL text containing the LIKE clause.</param>
    /// <returns>The parsed LIKE clause.</returns>
    public static LikeClause Parse(ValueBase value, string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(value, r, false);
    }

    /// <summary>
    /// Parses a LIKE clause from the token stream.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="r">The token reader.</param>
    /// <param name="isNegative">Specifies whether the LIKE clause is negated.</param>
    /// <returns>The parsed LIKE clause.</returns>
    public static LikeClause Parse(ValueBase value, ITokenReader r, bool isNegative)
    {
        r.Read("like");

        var argument = ValueParser.ParseCore(r);
        return new LikeClause(value, argument, isNegative);
    }
}
