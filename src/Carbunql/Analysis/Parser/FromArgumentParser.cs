using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a FROM argument from SQL text or token streams.
/// </summary>
public static class FromArgumentParser
{
    /// <summary>
    /// Parses a FROM argument from SQL text.
    /// </summary>
    /// <param name="unit">The unit value.</param>
    /// <param name="argument">The SQL text containing the FROM argument.</param>
    /// <returns>The parsed FROM argument.</returns>
    public static FromArgument Parse(string unit, string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(unit, r);
    }

    /// <summary>
    /// Parses a FROM argument from the token stream.
    /// </summary>
    /// <param name="unit">The unit value.</param>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed FROM argument.</returns>
    public static FromArgument Parse(string unit, ITokenReader r)
    {
        var value = ValueParser.Parse(r);
        return new FromArgument(unit, value);
    }
}
