using System.Collections;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Analysis;

public class Table<T> : IQueryable<T>
{
	public Table()
	{
		Query = Enumerable.Empty<T>().AsQueryable();
	}

	public Table(string tableName)
	{
		Query = new TableQuery<T>() { TableName = tableName };
	}

	public Table(IQueryable<T> subquery)
	{
		Query = new TableQuery<T>() { InnerQuery = subquery.AsQueryable() };
	}

	private IQueryable<T> Query { get; set; }

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
