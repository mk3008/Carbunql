using System.Collections;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

public abstract class TableQuery
{
	public string TableName { get; internal set; } = string.Empty;

	public IQueryable? InnerQuery { get; internal set; }

	public SelectQuery? InnerSelectQuery { get; internal set; }
}

public class TableQuery<T> : TableQuery, IOrderedQueryable<T>, IQueryProvider
{
	public TableQuery()
	{
		_expression = Expression.Constant(this);
	}

	private readonly Expression _expression;

	IQueryProvider IQueryable.Provider => this;

	Expression IQueryable.Expression => _expression;

	Type IQueryable.ElementType => typeof(T);

	IQueryable IQueryProvider.CreateQuery(Expression expression)
	{
		return new EnumerableQuery<T>(expression);
	}

	IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
	{
		return new EnumerableQuery<TElement>(expression);
	}

	object? IQueryProvider.Execute(Expression expression)
	{
		throw new NotImplementedException();
	}

	TElement IQueryProvider.Execute<TElement>(Expression expression)
	{
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		throw new NotImplementedException();
	}
}