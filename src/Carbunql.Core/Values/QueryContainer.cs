using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public abstract class QueryContainer : ValueBase
{
    public QueryContainer(IQueryCommandable query)
    {
        Query = query;
    }

    public IQueryCommandable Query { get; init; }
}