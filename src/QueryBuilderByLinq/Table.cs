using System.Collections;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

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
