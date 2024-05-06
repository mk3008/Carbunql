namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a Common Table Expression (CTE) query.
/// </summary>
public class CTEQueryParser
{
    /// <summary>
    /// Parses a CTE query from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed CTE query.</returns>
    internal static SelectQuery Parse(ITokenReader r)
    {
        var w = WithClauseParser.Parse(r);
        var sq = SelectQueryParser.Parse(r);
        sq.WithClause = w;
        return sq;
    }
}
