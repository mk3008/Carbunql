using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses WHEN expressions from SQL text or token stream.
/// </summary>
public class WhenExpressionParser
{
    /// <summary>
    /// Parses a WHEN expression from SQL text.
    /// </summary>
    /// <param name="text">The SQL text to parse.</param>
    /// <returns>The parsed WHEN expression.</returns>
    public static WhenExpression Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a WHEN expression from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed WHEN expression.</returns>
    public static WhenExpression Parse(ITokenReader r)
    {
        if (r.Peek().IsEqualNoCase("when"))
        {
            return ParseWhen(r);
        }
        else if (r.Peek().IsEqualNoCase("else"))
        {
            return ParseElse(r);
        }
        throw new SyntaxException($"Expects 'when', 'else'. (Actual: {r.Peek()})");
    }

    private static WhenExpression ParseWhen(ITokenReader r)
    {
        r.Read("when");
        var whenv = ValueParser.Parse(r);
        r.Read("then");
        var thenv = ValueParser.Parse(r);
        return new WhenExpression(whenv, thenv);
    }

    private static WhenExpression ParseElse(ITokenReader r)
    {
        r.Read("else");
        var v = ValueParser.Parse(r);
        return new WhenExpression(v);
    }
}
