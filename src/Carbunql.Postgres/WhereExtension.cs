using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

public class WhereExtension<T>
{
	internal WhereExtension(SelectQuery sourceQuery, SelectQuery argumentQuery)
	{
		SourceQuery = sourceQuery;
		ArgumentQuery = argumentQuery;
	}

	private SelectQuery SourceQuery { get; init; }

	private SelectQuery ArgumentQuery { get; init; }

	public void Exists(Expression<Func<T, bool>> predicate)
	{
		var tables = SourceQuery.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		ArgumentQuery.Where(v);

		SourceQuery.Where(new ExistsExpression(ArgumentQuery));
	}

	public void NotExists(Expression<Func<T, bool>> predicate)
	{
		var tables = SourceQuery.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		ArgumentQuery.Where(v);

		SourceQuery.Where(new NegativeValue(new ExistsExpression(ArgumentQuery)));
	}
}

public static class WhereExtension
{
	public static ValueBase Where(this SelectQuery source, Expression<Func<bool>> predicate)
	{
		var tables = source.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		if (v is BracketValue)
		{
			source.Where(v);
		}
		else
		{
			source.Where(new BracketValue(v));
		}

		return v;
	}

	public static WhereExtension<T> WhereAs<T>(this SelectQuery source, string alias)
	{
		return source.WhereAs<T>(typeof(T).ToTableName(), alias);
	}

	public static WhereExtension<T> WhereAs<T>(this SelectQuery source, string table, string alias)
	{
		var sq = new SelectQuery();
		sq.From(table).As(alias);
		sq.SelectAll();

		return new WhereExtension<T>(source, sq);
	}

	public static WhereExtension<T> WhereAs<T>(this SelectQuery source, Func<SelectQuery> subqueryBuilder, string alias)
	{
		var sq = new SelectQuery();
		sq.From(subqueryBuilder()).As(alias);
		sq.SelectAll();

		return new WhereExtension<T>(source, sq);
	}
}