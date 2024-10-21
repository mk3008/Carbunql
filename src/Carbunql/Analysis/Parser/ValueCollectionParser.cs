using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a collection of values from SQL text or token streams.
/// </summary>
public static class ValueCollectionParser
{
    /// <summary>
    /// Parses a collection of values from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the values.</param>
    /// <returns>The parsed collection of values.</returns>
    public static ValueCollection Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return new ValueCollection(ReadValues(r).ToList());
    }

    /// <summary>
    /// Parses a collection of values from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed collection of values.</returns>
    public static ValueCollection Parse(ITokenReader r)
    {
        return new ValueCollection(ReadValues(r).ToList());
    }

    /// <summary>
    /// Parses a collection of values from the token stream within parentheses.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed collection of values.</returns>
    public static ValueCollection ParseAsInner(ITokenReader r)
    {
        using var ir = new BracketInnerTokenReader(r);
        if (ir.Peek() == ")") return new ValueCollection();

        var v = new ValueCollection(ReadValues(ir).ToList());
        return v;
    }

    /// <summary>
    /// Reads values from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>An enumerable collection of values.</returns>
    internal static IEnumerable<ValueBase> ReadValues(ITokenReader r)
    {
        do
        {
            if (r.Peek().IsEqualNoCase(",")) r.Read();
            var v = ValueParser.Parse(r);

            if (r.ReadOrDefault("from") != null)
            {
                yield return FromArgumentParser.Parse(v.ToText(), r);
            }
            else if (r.ReadOrDefault("as") != null)
            {
                yield return AsArgumentParser.Parse(v, r);
            }
            else
            {
                yield return v;
            }
        }
        while (r.Peek().IsEqualNoCase(","));
    }
}
