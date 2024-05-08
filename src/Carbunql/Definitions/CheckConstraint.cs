using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a check constraint associated with a table.
/// </summary>
public class CheckConstraint : IConstraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckConstraint"/> class with the specified table.
    /// </summary>
    /// <param name="t">The table associated with the check constraint.</param>
    public CheckConstraint(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the name of the check constraint.
    /// </summary>
    public string ConstraintName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value representing the check constraint.
    /// </summary>
    public ValueBase Value { get; set; } = null!;

    /// <summary>
    /// Gets the name of the column associated with the check constraint (empty for check constraints at table level).
    /// </summary>
    public string ColumnName => string.Empty;

    /// <summary>
    /// Gets the schema of the table associated with the check constraint.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets the name of the table associated with the check constraint.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets the common tables associated with the check constraint (currently empty).
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the check constraint (currently empty).
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the check constraint (currently empty).
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the check constraint (currently empty).
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing the check constraint.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(ConstraintName))
        {
            yield return new Token(this, parent, "constraint", isReserved: true);
            yield return new Token(this, parent, ConstraintName);
        }

        yield return new Token(this, parent, "check", isReserved: true);

        foreach (var item in Value.GetTokens(parent))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Tries to apply the check constraint to the specified table definition clause (currently not implemented).
    /// </summary>
    public bool TrySet(TableDefinitionClause clause)
    {
        return false;
    }

    /// <summary>
    /// Attempts to disassemble the check constraint (currently returns the check constraint itself).
    /// </summary>
    /// <param name="constraint">The disassembled check constraint, which is the check constraint itself.</param>
    /// <returns>True if the check constraint was successfully disassembled; otherwise, false.</returns>
    public bool TryDisassemble([MaybeNullWhen(false)] out IConstraint constraint)
    {
        constraint = this;
        return true;
    }
}

