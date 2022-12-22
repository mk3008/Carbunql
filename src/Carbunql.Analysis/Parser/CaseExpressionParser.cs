using Carbunql.Core.Values;

namespace Carbunql.Analysis.Parser;

public static class CaseExpressionParser
{
    public static CaseExpression Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static CaseExpression Parse(TokenReader r)
    {
        r.ReadToken("case");

        var cndtext = r.ReadUntilToken("when");

        CaseExpression? c = null;
        if (string.IsNullOrEmpty(cndtext))
        {
            c = new CaseExpression();
        }
        else
        {
            var cnd = ValueParser.Parse(cndtext);
            c = new CaseExpression(cnd);
        }

        var exptext = r.ReadUntilToken("end");
        foreach (var w in WhenExpressionParser.Parse("when " + exptext + " end"))
        {
            c.WhenExpressions.Add(w);
        }
        return c;
    }
}