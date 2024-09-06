using Carbunql.Clauses;

namespace Carbunql.Tables;

/// <summary>
/// Represents a lateral table.
/// </summary>
public class LateralTable : TableBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LateralTable"/> class.
    /// </summary>
    public LateralTable()
    {
        Table = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LateralTable"/> class with the specified table.
    /// </summary>
    /// <param name="table">The table associated with the lateral table.</param>
    public LateralTable(TableBase table)
    {
        Table = table;
    }

    /// <summary>
    /// Gets or sets the table associated with the lateral table.
    /// </summary>
    public TableBase Table { get; init; }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "lateral");
        foreach (var item in Table.GetTokens(parent)) yield return item;
    }

    /// <inheritdoc/>
    public override IEnumerable<QueryParameter> GetParameters()
    {
        return Table.GetParameters();
    }

    /// <inheritdoc/>
    public override IList<string> GetColumnNames()
    {
        if (Table is IReadQuery q)
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
    public override bool IsSelectQuery => Table.IsSelectQuery;

    /// <inheritdoc/>
    public override SelectQuery GetSelectQuery()
    {
        return Table.GetSelectQuery();
    }

    /// <inheritdoc/>
    public override IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Table.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Table.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
    }
}
