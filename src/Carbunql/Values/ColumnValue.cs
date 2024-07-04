using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a column value.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class ColumnValue : ValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValue"/> class.
    /// </summary>
    public ColumnValue()
    {
        Column = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValue"/> class with the specified column name.
    /// </summary>
    /// <param name="column">The column name.</param>
    public ColumnValue(string column)
    {
        Column = column;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValue"/> class with the specified table alias and column name.
    /// </summary>
    /// <param name="table">The table alias.</param>
    /// <param name="column">The column name.</param>
    public ColumnValue(string table, string column)
    {
        TableAlias = table;
        Column = column;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValue"/> class with the specified FROM clause and column name.
    /// </summary>
    /// <param name="from">The FROM clause.</param>
    /// <param name="column">The column name.</param>
    public ColumnValue(FromClause from, string column)
    {
        TableAlias = from.Root.Alias;
        Column = column;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValue"/> class with the specified selectable table and column name.
    /// </summary>
    /// <param name="table">The selectable table.</param>
    /// <param name="column">The column name.</param>
    public ColumnValue(SelectableTable table, string column)
    {
        TableAlias = table.Alias;
        Column = column;
    }

    /// <summary>
    /// Gets or sets the table alias.
    /// </summary>
    public string TableAlias { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column name.
    /// </summary>
    public string Column { get; init; }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(TableAlias))
        {
            yield return new Token(this, parent, TableAlias);
            yield return Token.Dot(this, parent);
        }
        yield return new Token(this, parent, Column);
    }

    /// <inheritdoc/>
    public override string GetDefaultName()
    {
        if (OperatableValue == null) return Column;
        return string.Empty;
    }

    /// <inheritdoc/>
    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        yield break;
    }

    /// <inheritdoc/>
    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        yield break;
    }

    internal override IEnumerable<ColumnValue> GetColumnsCore()
    {
        yield return this;
    }
}
