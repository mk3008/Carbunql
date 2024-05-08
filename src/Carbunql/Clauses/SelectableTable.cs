using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a selectable table in a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class SelectableTable : IQueryCommandable, ISelectable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableTable"/> class with the specified table, alias, and column aliases.
    /// </summary>
    /// <param name="table">The table to be selected.</param>
    /// <param name="alias">The alias for the selected table.</param>
    public SelectableTable(TableBase table, string alias)
    {
        Table = table;
        Alias = alias;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableTable"/> class with the specified table, alias, and column aliases.
    /// </summary>
    /// <param name="table">The table to be selected.</param>
    /// <param name="alias">The alias for the selected table.</param>
    /// <param name="columnAliases">The column aliases for the selected table.</param>
    public SelectableTable(TableBase table, string alias, ValueCollection columnAliases)
    {
        Table = table;
        Alias = alias;
        ColumnAliases = columnAliases;
    }

    /// <summary>
    /// Gets the table to be selected.
    /// </summary>
    public TableBase Table { get; init; }

    /// <summary>
    /// Gets or sets the alias for the selected table.
    /// </summary>
    public string Alias { get; private set; }

    public void SetAlias(string alias)
    {
        this.Alias = alias;
    }

    /// <summary>
    /// Gets or sets the column aliases for the selected table.
    /// </summary>
    public ValueCollection? ColumnAliases { get; init; }

    public IEnumerable<Token> GetAliasTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(Alias) && Alias != Table.GetDefaultName())
        {
            yield return new Token(this, parent, Alias);
        }

        if (ColumnAliases != null)
        {
            var bracket = Token.ReservedBracketStart(this, parent);
            yield return bracket;
            foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
            yield return Token.ReservedBracketEnd(this, parent);
        }
    }

    /// <inheritdoc/>
    public virtual IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in Table.GetTokens(parent)) yield return item;

        if (!string.IsNullOrEmpty(Alias) && Alias != Table.GetDefaultName())
        {
            yield return Token.Reserved(this, parent, "as");
            yield return new Token(this, parent, Alias);
        }

        if (ColumnAliases != null)
        {
            var bracket = Token.ReservedBracketStart(this, parent);
            yield return bracket;
            foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
            yield return Token.ReservedBracketEnd(this, parent);
        }
    }

    /// <inheritdoc/>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Table.GetParameters())
        {
            yield return item;
        }
        var q = ColumnAliases?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
    }

    public IEnumerable<string> GetColumnNames()
    {
        if (ColumnAliases != null) return ColumnAliases.GetColumnNames();
        return Table.GetColumnNames();
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
    public virtual IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Table.GetCommonTables())
        {
            yield return item;
        }
    }
}
