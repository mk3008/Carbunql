using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class ValuesQueryParser
{
	public static ValuesQuery Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	internal static ValuesQuery Parse(ITokenReader r)
	{
		r.Read("values");

		var sq = ValuesClauseParser.Parse(r);
		sq.OrderClause = ParseOrderOrDefault(r);

		var tokens = new string[] { "union", "except", "minus", "intersect" };
		if (r.Peek().AreContains(tokens))
		{
			var op = r.Read();
			sq.AddOperatableValue(op, Parse(r));
		}

		sq.LimitClause = ParseLimitOrDefault(r);
		return sq;
	}

	private static OrderClause? ParseOrderOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("order") == null) return null;
		return OrderClauseParser.Parse(r);
	}

	private static LimitClause? ParseLimitOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("limit") == null) return null;
		return LimitClauseParser.Parse(r);
	}
}