using Carbunql.Definitions;

namespace Carbunql.Annotations;

/// <summary>
/// Attribute used to define metadata for a database table.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public class TableAttribute : Attribute, ITable
{
    /// <summary>
    /// Initializes a new instance of the TableAttribute class with the specified primary key identifiers.
    /// </summary>
    /// <param name="identifiers">The identifiers of the primary key properties.</param>
    public TableAttribute(string[] identifiers)
    {
        PrimaryKeyProperties = identifiers;
        if (identifiers.Length > 1) HasAutoNumber = false;
    }

    /// <summary>
    /// Gets or initializes the primary key properties of the table.
    /// </summary>
    public IEnumerable<string> PrimaryKeyProperties { get; init; }

    /// <summary>
    /// Gets or initializes the schema of the table.
    /// </summary>
    public string Schema { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes the name of the table.
    /// </summary>
    public string Table { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes the name of the constraint associated with the table.
    /// </summary>
    public string ConstraintName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes the comment associated with the table.
    /// </summary>
    public string Comment { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes a value indicating whether the table has an auto-incrementing primary key.
    /// </summary>
    public bool HasAutoNumber { get; init; } = true;

    /// <summary>
    /// Gets or initializes the definition for the auto-incrementing primary key.
    /// </summary>
    public string AutoNumberDefinition { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes the command to retrieve the next value of the auto-incrementing primary key.
    /// </summary>
    public string NextValueCommand { get; init; } = string.Empty;
}
