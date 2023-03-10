using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class LikeExpressionParser
{
	public static LikeExpression Parse(ValueBase value, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(value, r);
	}

	public static LikeExpression Parse(ValueBase value, ITokenReader r)
	{
		var argument = ValueParser.ParseCore(r);
		return new LikeExpression(value, argument);
	}
}