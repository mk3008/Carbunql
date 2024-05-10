using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents a query used for reading data from a database.
/// </summary>
[Union(0, typeof(SelectQuery))]
[Union(1, typeof(ValuesQuery))]
public abstract class ReadQuery : IReadQuery
{
    public abstract SelectClause? GetSelectClause();

    /// <summary>
    /// Gets or sets the list of operatable queries associated with this read query.
    /// </summary>
    public List<OperatableQuery> OperatableQueries { get; set; } = new();

    /// <summary>
    /// Gets or sets the order clause of the query.
    /// </summary>
    public OrderClause? OrderClause { get; set; }

    /// <summary>
    /// Gets or sets the limit clause of the query.
    /// </summary>
    public LimitClause? LimitClause { get; set; }

    /// <summary>
    /// Adds an operatable value to the query.
    /// </summary>
    /// <param name="operator">The operator of the operatable value.</param>
    /// <param name="query">The query associated with the operatable value.</param>
    /// <returns>The modified query.</returns>
    public IReadQuery AddOperatableValue(string @operator, IReadQuery query)
    {
        OperatableQueries.Add(new OperatableQuery(@operator, query));
        return query;
    }

    /// <inheritdoc/>
    public abstract IEnumerable<SelectQuery> GetInternalQueries();

    /// <inheritdoc/>
    public abstract IEnumerable<PhysicalTable> GetPhysicalTables();

    /// <inheritdoc/>
    public abstract IEnumerable<CommonTable> GetCommonTables();

    /// <inheritdoc/>
    public List<QueryParameter> Parameters { get; set; } = new();

    /// <inheritdoc/>
    public virtual IEnumerable<QueryParameter> GetInnerParameters()
    {
        yield break;
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        var q = GetWithClause()?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = GetSelectClause()?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = GetInnerParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        foreach (var oq in OperatableQueries)
        {
            foreach (var item in oq.GetParameters())
            {
                yield return item;
            }
        }
        q = OrderClause?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = LimitClause?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        foreach (var item in Parameters)
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public abstract IEnumerable<Token> GetCurrentTokens(Token? parent);

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in GetCurrentTokens(parent)) yield return item;
        foreach (var oq in OperatableQueries)
        {
            foreach (var item in oq.GetTokens(parent)) yield return item;
        }
        if (OrderClause != null) foreach (var item in OrderClause.GetTokens(parent)) yield return item;
        if (LimitClause != null) foreach (var item in LimitClause.GetTokens(parent)) yield return item;
    }

    public ReadQuery GetQuery()
    {
        return this;
    }

    /// <inheritdoc/>
    public abstract WithClause? GetWithClause();

    /// <inheritdoc/>
    public abstract SelectQuery GetOrNewSelectQuery();

    /// <inheritdoc/>
    public abstract IEnumerable<string> GetColumnNames();

    /// <inheritdoc/>
    public abstract SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases);

    /// <summary>
    /// Adds a parameter to the query.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="Value">The value of the parameter.</param>
    /// <returns>The name of the added parameter.</returns>
    public string AddParameter(string name, object? Value)
    {
        Parameters.Add(new QueryParameter(name, Value));
        return name;
    }
}
