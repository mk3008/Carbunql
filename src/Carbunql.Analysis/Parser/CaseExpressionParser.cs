﻿using Carbunql.Clauses;
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
		r.ReadOrDefault("case");

		if (r.Peek().AreEqual("when"))
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
		while (r.Peek().AreContains(new string[] { "when", "else" }));

		return lst;
	}
}