using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Analysis.Parser;

public static class AsExpressionParser
{
	public static AsExpression Parse(ValueBase value, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(value, r);
	}

	public static AsExpression Parse(ValueBase value, ITokenReader r)
	{
		var type = ValueParser.Parse(r);
		return new AsExpression(value, type);
	}
}
