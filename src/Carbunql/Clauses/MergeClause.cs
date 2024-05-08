using Carbunql.Tables;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause for merging data into a table in a SQL query.
/// </summary>
public class MergeClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MergeClause"/> class with the specified selectable table.
    /// </summary>
    /// <param name="table">The selectable table.</param>
    public MergeClause(SelectableTable table)
    {
        Table = table;
    }

    /// <summary>
    /// Gets the selectable table involved in the merge operation.
    /// </summary>
    public SelectableTable Table { get; init; }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Table.GetParameters();
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Table.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Table.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "merge into");
        yield return t;
        foreach (var item in Table.GetTokens(t)) yield return item;
    }
}
