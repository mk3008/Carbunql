using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class SelectQueryParser
{
	public static SelectQuery Parse(string text)
	{
		using var r = new TokenReader(text);

		if (r.Peek().IsEqualNoCase("with")) return CTEQueryParser.Parse(r);
		return Parse(r);
	}

	public static SelectQuery ParseAsInner(ITokenReader r)
	{
		r.Read("(");
		using var ir = new BracketInnerTokenReader(r);
		var v = Parse(ir);
		return v;
	}

	internal static SelectQuery Parse(ITokenReader r)
	{
		var sq = new SelectQuery();

		r.Read("select");

		sq.SelectClause = SelectClauseParser.Parse(r);
		sq.FromClause = ParseFromOrDefault(r);
		sq.WhereClause = ParseWhereOrDefault(r);
		sq.GroupClause = ParseGroupOrDefault(r);
		sq.HavingClause = ParseHavingOrDefault(r);
		sq.WindowClause = ParseWindowOrDefault(r);
		sq.OrderClause = ParseOrderOrDefault(r);

		var tokens = new string[] { "union", "union all", "except", "minus", "intersect" };
		if (r.Peek().IsEqualNoCase(tokens))
		{
			var op = r.Read();
			sq.AddOperatableValue(op, Parse(r));
		}

		sq.LimitClause = ParseLimitOrDefault(r);
		return sq;
	}

	private static FromClause? ParseFromOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("from") == null) return null;
		return FromClauseParser.Parse(r);
	}

	private static WhereClause? ParseWhereOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("where") == null) return null;
		return WhereClauseParser.Parse(r);
	}

	private static GroupClause? ParseGroupOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("group by") == null) return null;
		return GroupClauseParser.Parse(r);
	}

	private static HavingClause? ParseHavingOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("having") == null) return null;
		return HavingClauseParser.Parse(r);
	}
	private static WindowClause? ParseWindowOrDefault(ITokenReader r)
	{
		if (r.ReadOrDefault("window") == null) return null;
		return WindowClauseParser.Parse(r);
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