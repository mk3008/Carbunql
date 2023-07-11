using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class BetweenExpressionParser
{
	public static BetweenExpression Parse(ValueBase value, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(value, r, false);
	}

	public static BetweenExpression Parse(ValueBase value, ITokenReader r, bool isNegative)
	{
		var start = ValueParser.ParseCore(r);
		r.Read("and");
		var end = ValueParser.ParseCore(r);
		return new BetweenExpression(value, start, end, isNegative);
	}
}