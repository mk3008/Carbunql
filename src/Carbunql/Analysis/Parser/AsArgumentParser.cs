using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses an AS argument from SQL text or token streams.
/// </summary>
public static class AsArgumentParser
{
    /// <summary>
    /// Parses an AS argument from SQL text.
    /// </summary>
    /// <param name="value">The value to be parsed as an argument.</param>
    /// <param name="argument">The SQL text containing the AS argument.</param>
    /// <returns>The parsed AS argument.</returns>
    public static AsArgument Parse(ValueBase value, string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(value, r);
    }

    /// <summary>
    /// Parses an AS argument from the token stream.
    /// </summary>
    /// <param name="value">The value to be parsed as an argument.</param>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed AS argument.</returns>
    public static AsArgument Parse(ValueBase value, ITokenReader r)
    {
        var type = ValueParser.Parse(r);
        return new AsArgument(value, type);
    }
}
