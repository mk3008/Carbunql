using Carbunql;
using Carbunql.Clauses;
using System.Linq.Expressions;


namespace QueryBuilderByLinq;

public static class Sql
{
	/// <summary>
	/// This function returns a dual table.
	/// </summary>
	/// <returns>Returns an empty queryable object.</returns>
	public static IQueryable<object> Dual()
	{
		return Enumerable.Empty<object>().AsQueryable();
	}

	public static IQueryable<T> From<T>()
	{
		return Enumerable.Empty<T>().AsQueryable();
	}

	public static IQueryable<T> From<T>(string tableName)
	{
		return new Table<T>(tableName);
	}

	public static IQueryable<T> From<T>(IQueryable<T> subquery)
	{
		return new Table<T>(subquery);
	}

	public static IQueryable<T> InnerJoin<T>(Expression<Predicate<T>> condition)
	{
		return new Table<T>();
	}

	public static IQueryable<T> InnerJoin<T>(string tableName, Expression<Predicate<T>> condition)
	{
		return new Table<T>(tableName);
	}

	public static IQueryable<T> LeftJoin<T>(Expression<Predicate<T>> condition)
	{
		return new Table<T>();
	}

	public static IQueryable<T> LeftJoin<T>(string tableName, Expression<Predicate<T>> condition)
	{
		return new Table<T>(tableName);
	}

	public static IQueryable<T> CrossJoin<T>()
	{
		return Enumerable.Empty<T>().AsQueryable();
	}

	public static IQueryable<T> CrossJoin<T>(string tableName)
	{
		return new Table<T>(tableName);
	}

	private static string ERROR = "Definition methods must not be executed.";

	public static bool ExistsAs<T>(this SelectQuery source, string table, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool ExistsAs<T>(this SelectQuery source, IReadQuery subQuery, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool ExistsAs<T>(this SelectQuery source, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool InAs<T>(this SelectQuery source, string table, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool InAs<T>(this SelectQuery source, IReadQuery subQuery, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool InAs<T>(this SelectQuery source, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase Greatest(params object[] args)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase Least(params object[] args)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase RowNumber()
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase RowNumber(object? orderby)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase RowNumber(object? partitionby, object? orderby)
	{
		throw new InvalidProgramException(ERROR);
	}
}
