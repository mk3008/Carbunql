using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a casted value from SQL text or token streams.
/// </summary>
public static class CastValueParser
{
    /// <summary>
    /// Checks if the text represents a casted value.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text represents a casted value; otherwise, false.</returns>
    public static bool IsCastValue(string text)
    {
        return text == "::";
    }

    /// <summary>
    /// Parses a casted value from SQL text.
    /// </summary>
    /// <param name="value">The value to be casted.</param>
    /// <param name="symbol">The cast symbol (::).</param>
    /// <param name="argument">The SQL text containing the type of the casted value.</param>
    /// <returns>The parsed casted value.</returns>
    public static CastValue Parse(ValueBase value, string symbol, string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(value, symbol, r);
    }

    /// <summary>
    /// Parses a casted value from the token stream.
    /// </summary>
    /// <param name="value">The value to be casted.</param>
    /// <param name="symbol">The cast symbol (::).</param>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed casted value.</returns>
    public static CastValue Parse(ValueBase value, string symbol, ITokenReader r)
    {
        var type = ValueParser.ParseMain(r).ToText();

        if (r.Peek() == "[]")
        {
            type += r.Read("[]");
        }

        return new CastValue(value, symbol, type);
    }
}
