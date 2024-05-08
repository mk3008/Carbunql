using Carbunql.Clauses;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command for altering database schema.
/// </summary>
public interface IAlterCommand : IQueryCommandable, ITable
{
    /// <summary>
    /// Tries to apply the command to a table definition clause.
    /// </summary>
    /// <param name="clause">The table definition clause to which the command will be applied.</param>
    /// <returns><c>true</c> if the command was successfully applied; otherwise, <c>false</c>.</returns>
    bool TrySet(TableDefinitionClause clause);
}
