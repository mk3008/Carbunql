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

    /// <summary>
    /// Parses the READ query while ignoring surrounding brackets.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed ReadQuery object.</returns>
    internal static ReadQuery ParseIgnoringBrackets(ITokenReader r)
    {
        // Skip opening brackets
        var cnt = 0;
        while (r.TryRead("(", out _))
        {
            cnt++;
        }

        var q = Parse(r);

        // Skip closing brackets
        while (cnt > 0)
        {
            r.Read(")");
            cnt--;
        }

        return q;
    }
}
