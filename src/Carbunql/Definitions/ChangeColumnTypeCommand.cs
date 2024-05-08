using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command for changing the data type of a column in a table.
/// </summary>
public class ChangeColumnTypeCommand : IAlterCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeColumnTypeCommand"/> class with the specified table, column name, and column type.
    /// </summary>
    /// <param name="t">The table where the column resides.</param>
    /// <param name="columnName">The name of the column to be changed.</param>
    /// <param name="columnType">The new data type of the column.</param>
    public ChangeColumnTypeCommand(ITable t, string columnName, ValueBase columnType)
    {
        ColumnName = columnName;
        ColumnType = columnType;
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the name of the column to be changed.
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Gets or sets the new data type of the column.
    /// </summary>
    public ValueBase ColumnType { get; set; }

    /// <summary>
    /// Gets the schema of the table where the column resides.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets the name of the table where the column resides.
    /// </summary>
    public string Table { get; init; } = string.Empty;

    /// <summary>
    /// Gets the common tables associated with the command (currently empty).
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the command (currently empty).
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the command (currently empty).
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the command (currently empty).
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing the command for altering the column data type.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "alter column", isReserved: true);
        yield return new Token(this, parent, ColumnName);
        yield return new Token(this, parent, "type", isReserved: true);
        foreach (var item in ColumnType.GetTokens(parent))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Tries to apply the column data type change to the specified table definition clause.
    /// </summary>
    /// <param name="clause">The table definition clause to which the change will be applied.</param>
    /// <returns>True if the change was successfully applied; otherwise, false.</returns>
    public bool TrySet(TableDefinitionClause clause)
    {
        var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();

        c.ColumnType = ColumnType;
        return true;
    }

    /// <summary>
    /// Attempts to create an index query based on the column type change (currently not implemented).
    /// </summary>
    /// <param name="query">The created index query if successful; otherwise, default.</param>
    /// <returns>True if the index query was successfully created; otherwise, false.</returns>
    public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    {
        query = default;
        return false;
    }
}
