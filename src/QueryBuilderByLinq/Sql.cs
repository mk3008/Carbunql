using System.Collections;
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

public class Table<T> : IQueryable<T>
{
	private readonly IQueryable<T> Query = Enumerable.Empty<T>().AsQueryable();

	public Type ElementType => Query.ElementType;

	public Expression Expression => Query.Expression;

	public IQueryProvider Provider => Query.Provider;

	public IEnumerator<T> GetEnumerator()
	{
		return Query.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)Query).GetEnumerator();
	}
}
