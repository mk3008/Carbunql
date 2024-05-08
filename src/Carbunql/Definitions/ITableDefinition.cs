using Carbunql.Clauses;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a table definition.
/// </summary>
public interface ITableDefinition : IQueryCommandable, ITable
{
    /// <summary>
    /// Tries to set the table definition within the specified <paramref name="clause"/>.
    /// </summary>
    /// <param name="clause">The table definition clause.</param>
    /// <returns><see langword="true"/> if the table definition was successfully set; otherwise, <see langword="false"/>.</returns>
    bool TrySet(TableDefinitionClause clause);

    /// <summary>
    /// Tries to disassemble the table definition into a constraint.
    /// </summary>
    /// <param name="constraint">When this method returns, contains the disassembled constraint, if the conversion succeeded; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the table definition was successfully disassembled; otherwise, <see langword="false"/>.</returns>
    bool TryDisassemble([MaybeNullWhen(false)] out IConstraint constraint);

    /// <summary>
    /// Gets the name of the column associated with the table definition.
    /// </summary>
    string ColumnName { get; }
}