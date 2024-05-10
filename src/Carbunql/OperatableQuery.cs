using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents a query with an operator.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class OperatableQuery : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperatableQuery"/> class with the specified operator and query.
    /// </summary>
    /// <param name="operator">The operator, such as "union" or "union all".</param>
    /// <param name="query">The query to be operated upon.</param>
    public OperatableQuery(string @operator, IReadQuery query)
    {
        Operator = @operator;
        Query = query;
    }

    /// <summary>
    /// Gets or sets the operator used in the query.
    /// </summary>
    public string Operator { get; init; }

    /// <summary>
    /// Gets or sets the query to be operated upon.
    /// </summary>
    public IReadQuery Query { get; init; }

    /// <summary>
    /// Gets the parameters of the query.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Query.GetParameters();
    }

    /// <summary>
    /// Gets the tokens representing the query.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var current = Token.Reserved(this, parent, Operator);
        yield return current;
        foreach (var item in Query.GetTokens(current)) yield return item;
    }

    /// <summary>
    /// Gets the internal queries within the main query.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Query.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables involved in the query.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Query.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables involved in the query.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Query.GetCommonTables())
        {
            yield return item;
        }
    }
}
