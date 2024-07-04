using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a DELETE statement in SQL.
/// </summary>
public class DeleteClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteClause"/> class with the specified table.
    /// </summary>
    public DeleteClause(SelectableTable table)
    {
        Table = new SelectableTable(table.Table, table.Alias);
    }

    /// <summary>
    /// Gets the table from which rows will be deleted.
    /// </summary>
    public SelectableTable Table { get; init; }

    /// <summary>
    /// Gets the tokens representing this DELETE statement.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "delete from");
        yield return t;
        foreach (var item in Table.GetTokens(t)) yield return item;
    }

    /// <summary>
    /// Gets the internal queries associated with this DELETE statement.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Table.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables associated with this DELETE statement.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Table.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the parameters associated with this DELETE statement.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Table.GetParameters();
    }

    /// <summary>
    /// Gets the common tables associated with this DELETE statement.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}
