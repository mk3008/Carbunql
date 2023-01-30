using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Collections.Generic;

namespace Carbunql.Analysis.Parser;

public class WhenExpressionParser
{

	public static WhenExpression Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static WhenExpression Parse(ITokenReader r)
	{
		if (r.Peek().AreEqual("when"))
		{
			return ParseWhen(r);

		}
		else if (r.Peek().AreEqual("else"))
		{
			return ParseElse(r);
		}
		throw new SyntaxException($"expects 'when', 'else'.(actual : {r.Peek()})");
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