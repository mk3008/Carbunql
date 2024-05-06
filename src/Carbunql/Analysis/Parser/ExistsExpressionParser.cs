using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses an EXISTS expression from token streams.
/// </summary>
public static class ExistsExpressionParser
{
    /// <summary>
    /// Checks if the text represents an EXISTS expression.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text represents an EXISTS expression; otherwise, false.</returns>
    public static bool IsExistsExpression(string text)
    {
        return text.IsEqualNoCase("exists");
    }

    /// <summary>
    /// Parses an EXISTS expression from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed EXISTS expression.</returns>
    public static ExistsExpression Parse(ITokenReader r)
    {
        r.Read("exists");
        var q = SelectQueryParser.ParseAsInner(r);
        return new ExistsExpression(q);
    }
}
