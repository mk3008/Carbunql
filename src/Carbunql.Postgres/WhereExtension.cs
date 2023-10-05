using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

public class WhereExtension<T>
{
	internal WhereExtension(SelectQuery sourceQuery, SelectQuery argumentQuery, bool allowInject)
	{
		SourceQuery = sourceQuery;
		ArgumentQuery = argumentQuery;
		AllowInject = allowInject;
	}

	private SelectQuery SourceQuery { get; init; }

	private SelectQuery ArgumentQuery { get; init; }

	private bool AllowInject { get; init; }

	public void Exists(Expression<Func<T, bool>> predicate)
	{
		var tables = SourceQuery.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		ArgumentQuery.Where(v);

		if (AllowInject) SourceQuery.Where(new ExistsExpression(ArgumentQuery));
	}

	public void NotExists(Expression<Func<T, bool>> predicate)
	{
		var tables = SourceQuery.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		ArgumentQuery.Where(v);

		if (AllowInject) SourceQuery.Where(new NegativeValue(new ExistsExpression(ArgumentQuery)));
	}
}

public static class WhereExtension
{
	public static ValueBase Where(this SelectQuery source, Expression<Func<bool>> predicate)
	{
		var tables = source.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		source.Where(v);

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

		return new WhereExtension<T>(source, sq, allowInject: true);
	}

	public static WhereExtension<T> WhereAs<T>(this SelectQuery source, Func<SelectQuery> subqueryBuilder, string alias)
	{
		var sq = new SelectQuery();
		sq.From(subqueryBuilder()).As(alias);
		sq.SelectAll();

		return new WhereExtension<T>(source, sq, allowInject: true);
	}
}

public static class Sql
{
	public static bool ExistsAs<T>(this SelectQuery source, string alias, Expression<Func<T, bool>> predicate)
	{
		var sq = new SelectQuery();
		sq.From(typeof(T).ToTableName()).As(alias);
		sq.SelectAll();

		var we = new WhereExtension<T>(source, sq, allowInject: false);
		we.Exists(predicate);

		var v = new ExistsExpression(sq);
		return true;
	}

	public static bool ExistsAs<T>(string alias, Expression<Func<T, bool>> predicate)
	{
		return true;
	}
}
