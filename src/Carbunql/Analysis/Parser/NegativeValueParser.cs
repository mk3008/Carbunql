using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses negative values from token streams.
/// </summary>
public static class NegativeValueParser
{
    /// <summary>
    /// Determines if the given text represents a negative value.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text represents a negative value, otherwise false.</returns>
    public static bool IsNegativeValue(string text)
    {
        return text.IsEqualNoCase("not");
    }

    /// <summary>
    /// Parses a negative value from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed negative value.</returns>
    public static NegativeValue Parse(ITokenReader r)
    {
        r.Read("not");
        return new NegativeValue(ValueParser.Parse(r));
    }
}
