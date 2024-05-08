namespace Carbunql.Definitions;

/// <summary>
/// Represents a constraint applied to a database table.
/// </summary>
/// <remarks>
/// CONSTRAINT sale_pkey PRIMARY KEY (sale_id)
/// </remarks>
public interface IConstraint : ITableDefinition
{
    /// <summary>
    /// Gets the name of the constraint.
    /// </summary>
    string ConstraintName { get; }
}
