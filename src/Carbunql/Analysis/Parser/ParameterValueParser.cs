using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses parameter values from token streams.
/// </summary>
public static class ParameterValueParser
{
    /// <summary>
    /// Determines if the given text represents a parameter value.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text represents a parameter value, otherwise false.</returns>
    public static bool IsParameterValue(string text)
    {
        if (text.StartsWith("@@")) return false;
        return text.StartsWith(new string[] { ":", "@", "?" });
    }

    /// <summary>
    /// Parses a parameter value from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed parameter value.</returns>
    /// <exception cref="SyntaxException">Thrown when the token does not represent a parameter value.</exception>
    public static ParameterValue Parse(ITokenReader r)
    {
        var v = r.Peek();
        if (!IsParameterValue(v)) throw new SyntaxException($"Not a parameter: {v}");
        return new ParameterValue(r.Read());
    }
}
