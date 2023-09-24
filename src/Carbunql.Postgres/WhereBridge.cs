using Carbunql.Building;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

public class WhereBridge<T>
{
	internal WhereBridge(SelectQuery sourceQuery, SelectQuery argumentQuery)
	{
		SourceQuery = sourceQuery;
		ArgumentQuery = argumentQuery;
	}

	private SelectQuery SourceQuery { get; init; }

	private SelectQuery ArgumentQuery { get; init; }

	public void Exists(Expression<Func<T, bool>> predicate)
	{
		var v = predicate.Body.ToValue();

		ArgumentQuery.Where(v);

		SourceQuery.Where(new ExistsExpression(ArgumentQuery));
	}

	public void NotExists(Expression<Func<T, bool>> predicate)
	{
		var v = predicate.Body.ToValue();

		ArgumentQuery.Where(v);

		SourceQuery.Where(new NegativeValue(new ExistsExpression(ArgumentQuery)));
	}
}