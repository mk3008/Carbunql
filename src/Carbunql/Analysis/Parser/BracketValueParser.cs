using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a bracket value from token streams.
/// </summary>
public static class BracketValueParser
{
    /// <summary>
    /// Checks if the text represents a bracket value.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text represents a bracket value; otherwise, false.</returns>
    public static bool IsBracketValue(string text)
    {
        return text == "(";
    }

    public static bool TryParse(string text, [MaybeNullWhen(false)] out ValueBase value)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            value = null;
            return false;
        }
        value = q;
        return true;
    }

    /// <summary>
    /// Parses a bracket value from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed bracket value.</returns>
    public static ValueBase Parse(ITokenReader r)
    {
        using var ir = new BracketInnerTokenReader(r);

        var pt = ir.Peek();

        if (pt.IsEqualNoCase("select"))
        {
            var q = SelectQueryParser.Parse(ir);
            return new InlineQuery(q);
        }
        else
        {
            var v = ValueCollectionParser.Parse(ir);
            return new BracketValue(v);
        }
    }
}
