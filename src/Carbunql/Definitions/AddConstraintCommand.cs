using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

/// <summary>
/// Represents a command for adding a constraint to a table.
/// </summary>
public class AddConstraintCommand : IAlterCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddConstraintCommand"/> class with the specified constraint.
    /// </summary>
    /// <param name="constraint">The constraint to be added.</param>
    public AddConstraintCommand(IConstraint constraint)
    {
        Constraint = constraint;
    }

    /// <summary>
    /// Gets or sets the constraint to be added.
    /// </summary>
    public IConstraint Constraint { get; set; }

    /// <summary>
    /// Gets the schema of the table to which the constraint will be added.
    /// </summary>
    public string Schema => Constraint.Schema;

    /// <summary>
    /// Gets the name of the table to which the constraint will be added.
    /// </summary>
    public string Table => Constraint.Table;

    /// <summary>
    /// Gets the common tables associated with the constraint (currently empty).
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with the constraint (currently empty).
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with the constraint (currently empty).
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with the constraint (currently empty).
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing the constraint for altering the table structure.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return new Token(this, parent, "add", isReserved: true);
        foreach (var item in Constraint.GetTokens(parent))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Tries to apply the constraint to the specified table definition clause.
    /// </summary>
    /// <param name="clause">The table definition clause to which the constraint will be applied.</param>
    /// <returns>True if the constraint was successfully applied; otherwise, false.</returns>
    public bool TrySet(TableDefinitionClause clause)
    {
        return Constraint.TrySet(clause);
    }
}
