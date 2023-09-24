using Carbunql.Building;
using Carbunql.Clauses;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

public static class JoingBridgeExtension
{
	public static (Relation, T) InnerJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.InnerJoinAs<T>(table, alias);
	}

	public static (Relation, T) InnerJoinAs<T>(this FromClause source, string table, string alias)
	{
		return source.InnerJoin(table).As<T>(alias);
	}

	public static (Relation, T) LeftJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.LeftJoinAs<T>(table, alias);
	}

	public static (Relation, T) LeftJoinAs<T>(this FromClause source, string table, string alias)
	{
		return source.LeftJoin(table).As<T>(alias);
	}

	public static (Relation, T) RightJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.RightJoinAs<T>(table, alias);
	}

	public static (Relation, T) RightJoinAs<T>(this FromClause source, string table, string alias)
	{
		return source.RightJoin(table).As<T>(alias);
	}

	public static T CrossJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.CrossJoinAs<T>(table, alias);
	}

	public static T CrossJoinAs<T>(this FromClause source, string table, string alias)
	{
		var (_, t) = source.CrossJoin(table).As<T>(alias);
		return t;
	}

	public static T On<T>(this (Relation relation, T record) source, Expression<Func<T, bool>> predicate)
	{
		var v = predicate.Body.ToValue();

		source.relation.On((_) => v);

		return source.record;
	}
}
