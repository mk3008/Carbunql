using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class ValuesQueryParser
{
	public static ValuesQuery Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		var q = Parse(r);

		if (!r.Peek().IsEndToken())
		{
			throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
		}

		return q;
	}

	internal static ValuesQuery Parse(ITokenReader r)
	{
		r.Read("values");

		var sq = ValuesClauseParser.Parse(r);

		var tokens = new string[] { "union", "union all", "except", "minus", "intersect" };
		if (r.Peek().IsEqualNoCase(tokens))
		{
			var op = r.Read();
			sq.AddOperatableValue(op, Parse(r));
		}

		sq.OrderClause = ParseOrderOrDefault(r);
		sq.LimitClause = ParseLimitOrDefault(r);
		return sq;
	}

	private static OrderClause? ParseOrderOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("order by") == null) return null;
		return OrderClauseParser.Parse(r);
	}

	private static LimitClause? ParseLimitOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("limit") == null) return null;
		return LimitClauseParser.Parse(r);
	}
}