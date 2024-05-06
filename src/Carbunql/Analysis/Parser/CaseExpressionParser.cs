using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

/// <summary>
/// Parses a CASE expression from SQL text or token streams.
/// </summary>
public static class CaseExpressionParser
{
    /// <summary>
    /// Checks if the text represents a CASE expression.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if the text represents a CASE expression; otherwise, false.</returns>
    public static bool IsCaseExpression(string text)
    {
        return text.IsEqualNoCase("case");
    }

    /// <summary>
    /// Parses a CASE expression from SQL text.
    /// </summary>
    /// <param name="text">The SQL text containing the CASE expression.</param>
    /// <returns>The parsed CASE expression.</returns>
    public static CaseExpression Parse(string text)
    {
        var r = new SqlTokenReader(text);
        return Parse(r);
    }

    /// <summary>
    /// Parses a CASE expression from the token stream.
    /// </summary>
    /// <param name="r">The token reader.</param>
    /// <returns>The parsed CASE expression.</returns>
    public static CaseExpression Parse(ITokenReader r)
    {
        var exp = ParseCaseExpression(r);

        foreach (var w in ParseWhenExpressions(r))
        {
            exp.WhenExpressions.Add(w);
        }
        r.Read("end");

        return exp;
    }

    private static CaseExpression ParseCaseExpression(ITokenReader r)
    {
        r.Read("case");

        if (r.Peek().IsEqualNoCase("when"))
        {
            return new CaseExpression();
        }
        else
        {
            var v = ValueParser.Parse(r);
            return new CaseExpression(v);
        }
    }

    private static IEnumerable<WhenExpression> ParseWhenExpressions(ITokenReader r)
    {
        var lst = new List<WhenExpression>();
        do
        {
            lst.Add(WhenExpressionParser.Parse(r));
        }
        while (r.Peek().IsEqualNoCase(new string[] { "when", "else" }));

        return lst;
    }
}
