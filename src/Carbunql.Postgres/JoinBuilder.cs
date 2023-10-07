using Carbunql.Building;
using Carbunql.Clauses;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

public class JoinBuilder<T>
{
	public JoinBuilder(FromClause from, Relation relation)
	{
		From = from;
		Relation = relation;
	}

	internal FromClause From { get; init; }
	internal Relation Relation { get; init; }

	public T On(Expression<Func<T, bool>> predicate)
	{
		var tables = From.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		Relation.On((_) => v);

		return CreateDefinitionInstance();
	}

	public JoinBuilder<T> As(string Alias)
	{
		Relation.Table.SetAlias(Alias);
		return this;
	}

	public T CreateDefinitionInstance()
	{
		return (T)Activator.CreateInstance(typeof(T))!;
	}
}
