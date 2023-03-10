using Carbunql.Clauses;
using System.Diagnostics.Tracing;

namespace Carbunql.Building;

public static class QueryBaseExtension
{
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

	public static CommonTable With(this SelectQuery source, IReadQuery q)
	{
		source.ImportCommonTable(q);
		return source.With(q.ToCommonTable("cte"));
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

	public static CommonTable With(this SelectQuery source, Func<CommonTable> builder)
	{
		return source.With(builder());
	}

}