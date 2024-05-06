namespace Carbunql.Annotations;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]

/// <summary>
/// Attribute used to define an index on properties of a class.
/// </summary>
public class IndexAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the IndexAttribute class with the specified properties.
    /// </summary>
    /// <param name="properties">The properties to be included in the index.</param>
    public IndexAttribute(params string[] properties)
    {
        Properties = properties;
    }

    /// <summary>
    /// Gets or initializes the properties included in the index.
    /// </summary>
    public string[] Properties { get; init; }

    /// <summary>
    /// Gets or initializes the name of the index constraint.
    /// </summary>
    public string ConstraintName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes a value indicating whether the index is unique.
    /// </summary>
    public bool IsUnique { get; init; } = false;
}
