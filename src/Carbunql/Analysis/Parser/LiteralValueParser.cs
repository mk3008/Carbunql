using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses literal values from token streams.
/// </summary>
public static class LiteralValueParser
{
    /// <summary>
    /// Determines if the given text represents a literal value.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text represents a literal value, otherwise false.</returns>
    public static bool IsLiteralValue(string text)
    {
        return (text.IsNumeric() || text.StartsWith("'") || text.IsEqualNoCase("true") || text.IsEqualNoCase("false"));
    }

    /// <summary>
    /// Parses a literal value from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed literal value.</returns>
    public static LiteralValue Parse(ITokenReader r)
    {
        return new LiteralValue(r.Read());
    }
}
