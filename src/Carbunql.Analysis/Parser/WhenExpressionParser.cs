using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public class WhenExpressionParser
{

	public static List<WhenExpression> Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r).ToList();
	}

	public static IEnumerable<WhenExpression> Parse(ITokenReader r)
	{
		r.TryReadToken("when");
		var token = "when";

		while (token.AreEqual("when"))
		{
			var (x, y) = ParseWhenExpression(r);
			token = y;
			yield return x;
		}

		if (token.AreEqual("else"))
		{
			var val = Parse(r, "end");
			yield return new WhenExpression(val);
		}
	}

	private static (WhenExpression exp, string breaktoken) ParseWhenExpression(ITokenReader r)
	{
		var cnd = Parse(r, "then");
		var (val, breaktoken) = Parse(r, new string[] { "when", "else", "end" });
		var exp = new WhenExpression(cnd, val);
		return (exp, breaktoken);
	}

	private static ValueBase Parse(ITokenReader r, string endToken)
	{
		var ir = new InnerTokenReader(r, endToken);
		var val = ValueParser.Parse(ir);
		return val;
	}

	private static (ValueBase, string) Parse(ITokenReader r, IEnumerable<string> endTokens)
	{
		var ir = new InnerTokenReader(r, endTokens);
		var val = ValueParser.Parse(ir);
		var endToken = ir.TerminatedToken;
		return (val, endToken);
	}
}