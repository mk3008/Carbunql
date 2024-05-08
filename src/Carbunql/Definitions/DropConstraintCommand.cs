using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command to drop a constraint from a table.
/// </summary>
public class DropConstraintCommand : IAlterCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DropConstraintCommand"/> class with the specified table and constraint name.
    /// </summary>
    /// <param name="t">The table from which the constraint will be dropped.</param>
    /// <param name="constraintName">The name of the constraint to be dropped.</param>
    public DropConstraintCommand(ITable t, string constraintName)
    {
        ConstraintName = constraintName;
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the name of the constraint to be dropped.
    /// </summary>
    public string ConstraintName { get; set; }

    /// <summary>
    /// Gets or sets the schema of the table from which the constraint will be dropped.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table from which the constraint will be dropped.
    /// </summary>
    public string Table { get; init; } = string.Empty;

    /// <summary>
    /// Gets the common tables associated with the drop constraint command (currently empty).
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the drop constraint command (currently empty).
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the drop constraint command (currently empty).
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the drop constraint command (currently empty).
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing the drop constraint command.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "drop", isReserved: true);
        yield return new Token(this, parent, "constraint", isReserved: true);
        yield return new Token(this, parent, ConstraintName);
    }

    /// <summary>
    /// Attempts to apply the drop constraint command to a table definition clause.
    /// </summary>
    /// <param name="clause">The table definition clause to which the command will be applied.</param>
    /// <returns><c>true</c> if the command was successfully applied; otherwise, <c>false</c>.</returns>
    public bool TrySet(TableDefinitionClause clause)
    {
        return false;
    }

    /// <summary>
    /// Tries to convert the drop constraint command to a create index query.
    /// </summary>
    /// <param name="query">When this method returns, contains the create index query, if conversion succeeded; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the conversion succeeded; otherwise, <c>false</c>.</returns>
    public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
    {
        query = default;
        return false;
    }
}
