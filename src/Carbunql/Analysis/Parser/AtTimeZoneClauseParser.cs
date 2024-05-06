using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses an AT TIME ZONE clause from the token stream.
/// </summary>
public static class AtTimeZoneClauseParser
{
    /// <summary>
    /// Parses an AT TIME ZONE clause from the token stream.
    /// </summary>
    /// <param name="value">The value to be associated with the time zone.</param>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed AT TIME ZONE clause.</returns>
    public static AtTimeZoneClause Parse(ValueBase value, ITokenReader r)
    {
        r.Read("at time zone");
        var timezone = ValueParser.Parse(r);
        return new AtTimeZoneClause(value, timezone);
    }
}
