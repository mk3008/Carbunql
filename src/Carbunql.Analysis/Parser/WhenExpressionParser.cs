using Carbunql.Core.Extensions;
using Carbunql.Core.Values;

namespace Carbunql.Analysis.Parser;

public class WhenExpressionParser
{

    public static List<WhenExpression> Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r).ToList();
    }

    public static IEnumerable<WhenExpression> Parse(TokenReader r)
    {
        var token = r.ReadToken("when");

        while (token.AreEqual("when"))
        {
            var (x, y) = ParseWhenExpression(r);
            token = y;
            yield return x;
        }

        if (token.AreEqual("else"))
        {
            var val = ValueParser.Parse(r.ReadUntilToken("end"));
            yield return new WhenExpression(val);
        }
    }

    private static (WhenExpression exp, string breaktoken) ParseWhenExpression(TokenReader r)
    {
        var breaktokens = new string[] { "when", "else", "end" };
        var breaktoken = string.Empty;

        var fn = (string t) =>
        {
            if (t.AreContains(breaktokens))
            {
                breaktoken = t;
                return true;
            }
            return false;
        };

        var cnd = ValueParser.Parse(r.ReadUntilToken("then"));
        var val = ValueParser.Parse(r.ReadUntilToken(fn));
        var exp = new WhenExpression(cnd, val);
        return (exp, breaktoken);
    }
}