using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class CaseExpressionParser
{
	public static CaseExpression Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static CaseExpression Parse(ITokenReader r)
	{
		var c = ParseCaseExpression(r);

		var ir = new InnerTokenReader(r, "end");
		foreach (var w in WhenExpressionParser.Parse(ir))
		{
			c.WhenExpressions.Add(w);
		}
		return c;
	}

	private static CaseExpression ParseCaseExpression(ITokenReader r)
	{
		r.TryReadToken("case");

		var ir = new InnerTokenReader(r, "when");
		var t = ir.PeekRawToken();
		if (string.IsNullOrEmpty(t))
		{
			return new CaseExpression();
		}
		else
		{
			var cnd = ValueParser.Parse(ir);
			return new CaseExpression(cnd);
		}
	}
}