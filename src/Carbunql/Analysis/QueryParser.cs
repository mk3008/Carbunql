using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

/// <summary>
/// Provides functionality to parse various SQL queries.
/// </summary>
public static class QueryParser
{
    /// <summary>
    /// Parses the specified SQL query string.
    /// </summary>
    /// <param name="text">The SQL query string.</param>
    /// <returns>The parsed ReadQuery object.</returns>
    public static IReadQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens. (Token: '{r.Peek()}')");
        }

        return q;
    }

    /// <summary>
    /// Parses the SQL query using the provided ITokenReader.
    /// </summary>
    /// <param name="r">The ITokenReader instance.</param>
    /// <returns>The parsed ReadQuery object.</returns>
    public static IReadQuery Parse(ITokenReader r)
    {
        var token = r.Peek();
        if (token.IsEqualNoCase("with")) return CTEQueryParser.Parse(r);
        if (token.IsEqualNoCase("select")) return SelectQueryParser.Parse(r);
        if (token.IsEqualNoCase("values")) return ValuesQueryParser.Parse(r);

        throw new NotSupportedException($"Unsupported token: '{r.Peek()}'");
    }
}
