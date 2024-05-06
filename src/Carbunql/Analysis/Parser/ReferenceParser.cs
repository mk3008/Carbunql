using Carbunql.Analysis.Parser;
using Carbunql.Definitions;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Parses reference definitions from SQL text or token streams.
/// </summary>
public static class ReferenceParser
{
    /// <summary>
    /// Parses a reference definition from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the reference definition.</param>
    /// <returns>The parsed reference definition.</returns>
    public static ReferenceDefinition Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);
        return q;
    }

    /// <summary>
    /// Parses a reference definition from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed reference definition.</returns>
    public static ReferenceDefinition Parse(ITokenReader r)
    {
        var token = r.Read("references");
        var table = r.Read();
        var columns = ArrayParser.Parse(r);

        var option = string.Empty;
        if (r.Peek().IsEqualNoCase("on"))
        {
            r.Read();
            option = r.Read();
        }

        return new ReferenceDefinition(table, columns) { Option = option };
    }
}
