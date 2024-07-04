using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a container for a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class QueryContainer : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryContainer"/> class.
    /// </summary>
    public QueryContainer()
    {
        Query = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryContainer"/> class with the specified query.
    /// </summary>
    /// <param name="query">The query to be contained.</param>
    public QueryContainer(IQueryCommandable query)
    {
        Query = query;
    }

    /// <summary>
    /// Gets or sets the query contained within the container.
    /// </summary>
    public IQueryCommandable Query { get; init; }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        foreach (var item in Query.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        return Query.GetParameters();
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        foreach (var item in Query.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        foreach (var item in Query.GetCommonTables())
        {
            yield return item;
        }
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        foreach (var item in Query.GetColumns())
        {
            yield return item;
        }
    }
}
