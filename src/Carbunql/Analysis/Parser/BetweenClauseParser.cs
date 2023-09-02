using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class BetweenClauseParser
{
	public static BetweenClause Parse(ValueBase value, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(value, r, false);
	}

	public static BetweenClause Parse(ValueBase value, ITokenReader r, bool isNegative)
	{
		var start = ValueParser.ParseCore(r);
		r.Read("and");
		var end = ValueParser.ParseCore(r);
		return new BetweenClause(value, start, end, isNegative);
	}
}