using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses an array value from SQL text or token streams.
/// </summary>
public static class ArrayValueParser
{
    /// <summary>
    /// Parses an array value from SQL text.
    /// </summary>
    /// <param name="argument">The SQL text containing the array value.</param>
    /// <returns>The parsed array value.</returns>
    public static ValueBase Parse(string argument)
    {
        var r = new SqlTokenReader(argument);
        return Parse(r);
    }

    /// <summary>
    /// Parses an array value from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed array value.</returns>
    public static ValueBase Parse(ITokenReader r)
    {
        var token = r.Read("array");
        var next = r.Peek();
        if (next.First() == '[' && next.Last() == ']')
        {
            // It is interpreted as a SQL Server token, so disassemble it
            next = r.Read();
            var text = next.Substring(1, next.Length - 2);
            var value = ValueCollectionParser.Parse(text);
            return new ArrayValue(value);
        }
        else if (BracketValueParser.IsBracketValue(next))
        {
            var value = BracketValueParser.Parse(r);
            return new ArrayValue(value);
        }
        throw new NotSupportedException($"Unsupported token:{next}");
    }
}
