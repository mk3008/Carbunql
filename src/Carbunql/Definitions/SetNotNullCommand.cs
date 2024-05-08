using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command to set a column as not nullable.
/// </summary>
public class SetNotNullCommand : IAlterCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SetNotNullCommand"/> class with the specified table and column name.
    /// </summary>
    public SetNotNullCommand(ITable t, string columnName)
    {
        ColumnName = columnName;
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the name of the column.
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// Gets or sets the schema of the table.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table.
    /// </summary>
    public string Table { get; init; } = string.Empty;

    /// <summary>
    /// Gets the common tables associated with the command.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the command.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the command.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the command.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Generates tokens for the command.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "alter column", isReserved: true);
        yield return new Token(this, parent, ColumnName);
        yield return new Token(this, parent, "set", isReserved: true);
        yield return new Token(this, parent, "not null", isReserved: true);
    }

    /// <summary>
    /// Attempts to apply the command to a table definition clause.
    /// </summary>
    public bool TrySet(TableDefinitionClause clause)
    {
        var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();
        c.IsNullable = false;
        return true;
    }

    /// <summary>
    /// Attempts to create an index query based on the command.
    /// </summary>
    public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    {
        query = default;
        return false;
    }
}
