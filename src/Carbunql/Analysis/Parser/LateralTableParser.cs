using Carbunql.Tables;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a lateral table from the token stream.
/// </summary>
public class LateralTableParser
{
    /// <summary>
    /// Parses a lateral table from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed lateral table.</returns>
    public static LateralTable Parse(ITokenReader r)
    {
        r.Read("lateral");
        var t = new LateralTable(TableParser.Parse(r));
        return t;
    }
}
