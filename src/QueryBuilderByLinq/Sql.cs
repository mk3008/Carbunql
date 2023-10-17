using System.Linq.Expressions;

namespace QueryBuilderByLinq;

public static class Sql
{
	public static IQueryable<T> From<T>()
	{
		return Enumerable.Empty<T>().AsQueryable();
	}

	public static IQueryable<T> InnerJoin<T>(Expression<Predicate<T>> condition)
	{
		return new Table<T>();
	}

	public static IQueryable<T> LeftJoin<T>(Expression<Predicate<T>> condition)
	{
		return new Table<T>();
	}

	public static IQueryable<T> CrossJoin<T>(Expression<Predicate<T>> condition)
	{
		return new Table<T>();
	}
}
