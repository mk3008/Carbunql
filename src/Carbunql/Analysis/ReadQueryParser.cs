using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Provides functionality to parse READ queries in SQL.
/// </summary>
public static class ReadQueryParser
{
    /// <summary>
    /// Parses the specified SQL query string.
    /// </summary>
    /// <param name="text">The SQL query string.</param>
    /// <returns>The parsed ReadQuery object.</returns>
    public static ReadQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses the READ query using the provided ITokenReader.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed ReadQuery object.</returns>
    public static ReadQuery Parse(ITokenReader r)
    {
        if (r.Peek().IsEqualNoCase("select")) return SelectQueryParser.Parse(r);
        if (r.Peek().IsEqualNoCase("values")) return ValuesQueryParser.Parse(r);

        throw new NotSupportedException($"Unsupported token: '{r.Peek()}'");
    }
}
