using Carbunql.Clauses;

namespace Carbunql.Values;

public abstract class QueryContainer : ValueBase
{
	public QueryContainer(IQueryCommandable query)
	{
		Query = query;
	}

	public IQueryCommandable Query { get; init; }
}