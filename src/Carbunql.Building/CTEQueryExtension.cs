using Carbunql.Clauses;

namespace Carbunql.Building;

public static class QueryBaseExtension
{
	public static CommonTable With(this CTEQuery source, SelectQuery q)
	{
		return source.With(q.ToCommonTable("cte"));
	}

	public static CommonTable With(this CTEQuery source, Func<SelectQuery> builder)
	{
		return source.With(builder());
	}

	public static CommonTable With(this CTEQuery source, CommonTable ct)
	{
		source.WithClause.Add(ct);
		return ct;
	}

	public static CommonTable With(this CTEQuery source, Func<CommonTable> builder)
	{
		return source.With(builder());
	}

	public static CTEQuery ToCTE(this CTEQuery source, string alias)
	{
		var sq = new CTEQuery();

		foreach (var item in source.WithClause.CommonTables)
		{
			sq.WithClause.Add(item);
		}

		if (source.Query != null)
		{
			sq.WithClause.Add(source.Query.ToCommonTable(alias));
		}

		return sq;
	}

	public static SelectQuery GetOrNewSelectQuery(this CTEQuery source)
	{
		source.Query ??= new SelectQuery();
		return source.Query;
	}

	public static (CTEQuery, FromClause) ToSubQuery(this CTEQuery source, string alias)
	{
		if (source.Query == null) throw new NullReferenceException(nameof(source.Query));

		var sq = new SelectQuery();
		var (f, _) = sq.From(source.Query).As(alias);

		var cteq = new CTEQuery();
		foreach (var item in source.WithClause.CommonTables)
		{
			cteq.WithClause.Add(item);
		}
		cteq.Query = sq;
		return (cteq, f);
	}
}