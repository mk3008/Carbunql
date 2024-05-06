using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses WITHOUT TIME ZONE clauses from SQL token streams.
/// </summary>
public static class WithoutTimeZoneClauseParser
{
    /// <summary>
    /// Parses a WITHOUT TIME ZONE clause from the token stream.
    /// </summary>
    /// <param name="value">The value to which the WITHOUT TIME ZONE clause applies.</param>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed WITHOUT TIME ZONE clause.</returns>
    public static WithoutTimeZoneClause Parse(ValueBase value, ITokenReader r)
    {
        r.Read("without time zone");
        return new WithoutTimeZoneClause(value);
    }
}
