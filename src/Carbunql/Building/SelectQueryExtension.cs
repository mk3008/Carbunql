using Carbunql.Clauses;

namespace Carbunql.Building;

public static class QueryBaseExtension
{
	[Obsolete("With clauses do not need to be manually imported.")]
	public static SelectQuery ImportCommonTable(this SelectQuery source, IReadQuery tagert)
	{
		var w = tagert.GetWithClause();
		if (w == null) return source;

		source.WithClause ??= new WithClause();
		foreach (var item in w)
		{
			source.WithClause.Add(item);
		}
		return source;
	}

	public static CommonTable With(this SelectQuery source, string query)
	{
		var sq = new SelectQuery(query);
		return source.With(sq.ToCommonTable("cte"));
	}

	public static CommonTable With(this SelectQuery source, IReadQuery query)
	{
		return source.With(query.ToCommonTable("cte"));
	}

	public static CommonTable With(this SelectQuery source, Func<SelectQuery> builder)
	{
		return source.With(builder());
	}

	public static CommonTable With(this SelectQuery source, CommonTable ct)
	{
		source.WithClause ??= new WithClause();
		source.WithClause.Add(ct);
		return ct;
	}

	public static CommonTable With(this SelectQuery source, ValuesQuery q, IEnumerable<string> columnAliases)
	{
		return source.With(q.ToCommonTable("cte", columnAliases));
	}

	public static CommonTable With(this SelectQuery source, Func<CommonTable> builder)
	{
		return source.With(builder());
	}
}