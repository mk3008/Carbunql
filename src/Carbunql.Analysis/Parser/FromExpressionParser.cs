using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class FromExpressionParser
{
	public static FromExpression Parse(ValueBase unit, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(unit, r);
	}

	public static FromExpression Parse(ValueBase unit, ITokenReader r)
	{
		var value = ValueParser.Parse(r);
		return new FromExpression(unit, value);
	}
}
