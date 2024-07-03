using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command to rename a table.
/// </summary>
public class RenameTableCommand : IAlterCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenameTableCommand"/> class with the new table name.
    /// </summary>
    public RenameTableCommand(ITable t, string newTableName)
    {
        NewTableName = newTableName;
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the new name of the table.
    /// </summary>
    public string NewTableName { get; set; }

    /// <summary>
    /// Gets or sets the schema of the table.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table.
    /// </summary>
    public string Table { get; init; } = string.Empty;

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }

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
        yield return new Token(this, parent, "rename", isReserved: true);
        yield return new Token(this, parent, "to", isReserved: true);
        yield return new Token(this, parent, NewTableName);
    }

    /// <summary>
    /// Attempts to apply the command to a table definition clause.
    /// </summary>
    public bool TrySet(TableDefinitionClause clause)
    {
        return false;
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
