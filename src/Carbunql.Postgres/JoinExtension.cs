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

public static class JoinExtension
{
	public static JoinBuilder<T> InnerJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.InnerJoinAs<T>(table, alias);
	}

	public static JoinBuilder<T> InnerJoinAs<T>(this FromClause source, string table, string alias)
	{
		var r = source.InnerJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> InnerJoinAs<T>(this FromClause source, CommonTable table, string alias)
	{
		var r = source.InnerJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> InnerJoinAs<T>(this FromClause source, SelectableTable table, string alias)
	{
		var r = source.InnerJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> InnerJoinAs<T>(this FromClause source, IReadQuery query, string alias)
	{
		var r = source.InnerJoin(query);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> InnerJoinAs<T>(this FromClause source, Func<SelectQuery> builder, string alias)
	{
		return source.InnerJoinAs<T>(builder(), alias);
	}

	public static JoinBuilder<T> LeftJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.LeftJoinAs<T>(table, alias);
	}

	public static JoinBuilder<T> LeftJoinAs<T>(this FromClause source, string table, string alias)
	{
		var r = source.LeftJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> LeftJoinAs<T>(this FromClause source, CommonTable table, string alias)
	{
		var r = source.LeftJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> LeftJoinAs<T>(this FromClause source, SelectableTable table, string alias)
	{
		var r = source.LeftJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> LeftJoinAs<T>(this FromClause source, IReadQuery query, string alias)
	{
		var r = source.LeftJoin(query);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> LeftJoinAs<T>(this FromClause source, Func<SelectQuery> builder, string alias)
	{
		return source.LeftJoinAs<T>(builder(), alias);
	}

	public static JoinBuilder<T> RightJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.RightJoinAs<T>(table, alias);
	}

	public static JoinBuilder<T> RightJoinAs<T>(this FromClause source, string table, string alias)
	{
		var r = source.RightJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> RightJoinAs<T>(this FromClause source, CommonTable table, string alias)
	{
		var r = source.RightJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> RightJoinAs<T>(this FromClause source, SelectableTable table, string alias)
	{
		var r = source.RightJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> RightJoinAs<T>(this FromClause source, IReadQuery query, string alias)
	{
		var r = source.RightJoin(query);
		var builder = new JoinBuilder<T>(source, r);
		return builder.As(alias);
	}

	public static JoinBuilder<T> RightJoinAs<T>(this FromClause source, Func<SelectQuery> builder, string alias)
	{
		return source.RightJoinAs<T>(builder(), alias);
	}

	public static T CrossJoinAs<T>(this FromClause source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.CrossJoinAs<T>(table, alias);
	}

	public static T CrossJoinAs<T>(this FromClause source, string table, string alias)
	{
		var r = source.CrossJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		builder.As(alias);
		return builder.CreateDefinitionInstance();
	}

	public static T CrossJoinAs<T>(this FromClause source, CommonTable table, string alias)
	{
		var r = source.CrossJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		builder.As(alias);
		return builder.CreateDefinitionInstance();

	}

	public static T CrossJoinAs<T>(this FromClause source, SelectableTable table, string alias)
	{
		var r = source.CrossJoin(table);
		var builder = new JoinBuilder<T>(source, r);
		builder.As(alias);
		return builder.CreateDefinitionInstance();
	}

	public static T CrossJoinAs<T>(this FromClause source, IReadQuery query, string alias)
	{
		var r = source.CrossJoin(query);
		var builder = new JoinBuilder<T>(source, r);
		builder.As(alias);
		return builder.CreateDefinitionInstance();
	}

	public static T CrossJoinAs<T>(this FromClause source, Func<SelectQuery> builder, string alias)
	{
		return source.CrossJoinAs<T>(builder(), alias);
	}
}
