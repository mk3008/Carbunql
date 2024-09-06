using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

/// <summary>
/// Abstract base class for reading data.
/// Inherit from this class to implement selection queries or value specification queries.
/// </summary>
/// <remarks>
/// A selection query is a query used to retrieve data from a database.
/// A values query is a query used to specify values.
/// </remarks>
public abstract class ReadQuery : IReadQuery
{
    public abstract SelectClause? GetSelectClause();

    /// <summary>
    /// Gets or sets the list of operatable queries associated with this read query.
    /// Operatable queries, such as UNION or UNION ALL, are used to combine multiple read queries.
    /// </summary>
    public List<OperatableQuery> OperatableQueries { get; set; } = new();

    /// <summary>
    /// Gets or sets the ORDER BY clause of the query.
    /// The ORDER BY clause is used to sort the result set in ascending or descending order.
    /// </summary>
    public OrderClause? OrderClause { get; set; }

    /// <summary>
    /// Gets or sets the LIMIT clause of the query.
    /// The LIMIT clause is used to constrain the number of rows returned by the query.
    /// </summary>
    public LimitClause? LimitClause { get; set; }

    /// <summary>
    /// Adds an operatable query to the read query.
    /// Operatable queries, such as UNION or UNION ALL, are used to combine multiple read queries.
    /// </summary>
    /// <param name="operatorString">The operator used to combine queries. Please specify the operator as a string, such as "union" or "union all".</param>
    /// <param name="query">The query associated with the operatable value.</param>
    /// <returns>The modified read query with the added operatable query.</returns>
    public IReadQuery AddOperatableValue(string operatorString, IReadQuery query)
    {
        OperatableQueries.Add(new OperatableQuery(operatorString, query));
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

        // root or group
        if (parent == null || parent.Text == "(")
        {
            foreach (var oq in GetOperatableQueries())
            {
                foreach (var item in oq.GetTokens(parent)) yield return item;
            }
        }

        if (OrderClause != null) foreach (var item in OrderClause.GetTokens(parent)) yield return item;
        if (LimitClause != null) foreach (var item in LimitClause.GetTokens(parent)) yield return item;
    }

    /// <inheritdoc/>
    public IEnumerable<OperatableQuery> GetOperatableQueries()
    {
        foreach (var op in OperatableQueries)
        {
            yield return op;
            foreach (var subop in op.Query.GetOperatableQueries())
            {
                yield return subop;
            }
        }
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
    /// Adds a parameter to the parameter query.
    /// Parameters are used to dynamically inject values into the query, typically to prevent SQL injection attacks.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>The name of the added parameter.</returns>
    internal string AddParameter(string name, object? Value)
    {
        Parameters.Add(new QueryParameter(name, Value));
        return name;
    }

    public virtual IEnumerable<ColumnValue> GetColumns()
    {
        var q = GetSelectClause()?.GetColumns();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = OrderClause?.GetColumns();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
        q = LimitClause?.GetColumns();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
    }
}
