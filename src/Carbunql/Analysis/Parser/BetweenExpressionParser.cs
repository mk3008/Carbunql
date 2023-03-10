using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class BetweenExpressionParser
{
	public static BetweenExpression Parse(ValueBase value, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(value, r);
	}

	public static BetweenExpression Parse(ValueBase value, ITokenReader r)
	{
		var start = ValueParser.ParseCore(r);
		r.Read("and");
		var end = ValueParser.ParseCore(r);
		return new BetweenExpression(value, start, end);
	}
}