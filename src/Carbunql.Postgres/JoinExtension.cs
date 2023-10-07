using Carbunql.Building;
using Carbunql.Clauses;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Carbunql.Postgres;

public static class JoinExtension
{
	private static T AddRelation<T>(this FromClause source, Func<Relation> relationCreator, Expression<Func<T, bool>> predicate)
	{
		var alias = predicate.Parameters[0].Name;
		var tables = source.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		if (string.IsNullOrEmpty(alias)) throw new NullReferenceException(nameof(alias));

		var relation = relationCreator();
		relation.As(alias);
		relation.On((_) => predicate.Body.ToValue(tables));

		return (T)Activator.CreateInstance(typeof(T))!;
	}

	public static T InnerJoinAs<T>(this FromClause source, Expression<Func<T, bool>> predicate)
	{
		var table = typeof(T).ToTableName();
		return source.AddRelation(() => source.InnerJoin(table), predicate);
	}

	public static T InnerJoinAs<T>(this FromClause source, string table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.InnerJoin(table), predicate);
	}

	public static T InnerJoinAs<T>(this FromClause source, CommonTable table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.InnerJoin(table), predicate);
	}

	public static T InnerJoinAs<T>(this FromClause source, SelectableTable table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.InnerJoin(table), predicate);
	}

	public static T InnerJoinAs<T>(this FromClause source, IReadQuery query, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.InnerJoin(query), predicate);
	}

	public static T InnerJoinAs<T>(this FromClause source, Func<SelectQuery> builder, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.InnerJoin(builder()), predicate);
	}

	public static T LeftJoinAs<T>(this FromClause source, Expression<Func<T, bool>> predicate)
	{
		var table = typeof(T).ToTableName();
		return source.AddRelation(() => source.LeftJoin(table), predicate);
	}

	public static T LeftJoinAs<T>(this FromClause source, string table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.LeftJoin(table), predicate);
	}

	public static T LeftJoinAs<T>(this FromClause source, CommonTable table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.LeftJoin(table), predicate);
	}

	public static T LeftJoinAs<T>(this FromClause source, SelectableTable table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.LeftJoin(table), predicate);
	}

	public static T LeftJoinAs<T>(this FromClause source, IReadQuery query, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.LeftJoin(query), predicate);
	}

	public static T LeftJoinAs<T>(this FromClause source, Func<SelectQuery> builder, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.LeftJoin(builder()), predicate);
	}

	public static T RightJoinAs<T>(this FromClause source, Expression<Func<T, bool>> predicate)
	{
		var table = typeof(T).ToTableName();
		return source.AddRelation(() => source.RightJoin(table), predicate);
	}

	public static T RightJoinAs<T>(this FromClause source, string table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.RightJoin(table), predicate);
	}

	public static T RightJoinAs<T>(this FromClause source, CommonTable table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.RightJoin(table), predicate);
	}

	public static T RightJoinAs<T>(this FromClause source, SelectableTable table, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.RightJoin(table), predicate);
	}

	public static T RightJoinAs<T>(this FromClause source, IReadQuery query, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.RightJoin(query), predicate);
	}

	public static T RightJoinAs<T>(this FromClause source, Func<SelectQuery> builder, Expression<Func<T, bool>> predicate)
	{
		return source.AddRelation(() => source.RightJoin(builder()), predicate);
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
