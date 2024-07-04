using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause used to specify the table to be updated in an UPDATE statement.
/// </summary>
public class UpdateClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateClause"/> class with the specified table.
    /// </summary>
    /// <param name="table">The table to be updated.</param>
    public UpdateClause(SelectableTable table)
    {
        Table = new SelectableTable(table.Table, table.Alias);
    }

    /// <summary>
    /// Gets the table to be updated.
    /// </summary>
    public SelectableTable Table { get; init; }

    /// <summary>
    /// Gets the tokens representing the UPDATE clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "update");
        yield return t;
        foreach (var item in Table.GetTokens(t)) yield return item;
    }

    /// <summary>
    /// Gets the internal queries used in the clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Table.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables used in the clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Table.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables used in the clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the parameters used in the clause.
    /// </summary>
    /// <remarks>This method is not implemented and throws a <see cref="NotImplementedException"/>.</remarks>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}
