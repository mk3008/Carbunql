using Carbunql.Clauses;

namespace Carbunql.Tables;

/// <summary>
/// Represents a virtual table, which wraps a subquery and behaves like a table.
/// </summary>
public class VirtualTable : TableBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualTable"/> class.
    /// </summary>
    public VirtualTable()
    {
        _isSelectQuery = false;
        Query = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualTable"/> class with the specified select query.
    /// </summary>
    /// <param name="query">The select query.</param>
    public VirtualTable(SelectQuery query)
    {
        _isSelectQuery = true;
        Query = query;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualTable"/> class with the specified query.
    /// </summary>
    /// <param name="query">The query. It can be a select query, Values query, or any other query type.</param>
    public VirtualTable(IQueryCommandable query)
    {
        _isSelectQuery = query is SelectQuery;
        Query = query;
    }

    private readonly bool _isSelectQuery;

    /// <summary>
    /// Gets or sets the query associated with the virtual table.
    /// </summary>
    public IQueryCommandable Query { get; init; }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetParameters()
    {
        return Query.GetParameters();
    }

    /// <inheritdoc/>
    public override IList<string> GetColumnNames()
    {
        if (Query is IReadQuery q)
        {
            var s = q.GetOrNewSelectQuery().SelectClause;
            if (s == null) return base.GetColumnNames();
            return s.Select(x => x.Alias).ToList();
        }
        else
        {
            return base.GetColumnNames();
        }
    }

    /// <inheritdoc/>
    public override bool IsSelectQuery => IsSelectQueryCore();

    internal bool IsSelectQueryCore()
    {
        if (_isSelectQuery) return true;
        if (Query is VirtualTable vt) return vt.IsSelectQueryCore();

        return false;
    }

    /// <inheritdoc/>
    public override SelectQuery GetSelectQuery()
    {
        if (_isSelectQuery) return (SelectQuery)Query;
        if (Query is VirtualTable vt) return vt.GetSelectQuery();

        return base.GetSelectQuery();
    }

    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Query.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Query.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Query.GetCommonTables())
        {
            yield return item;
        }
    }
}
