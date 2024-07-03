namespace Carbunql.Definitions;

/// <summary>
/// Represents a mapping between a column name and a property name.
/// </summary>
public readonly struct PrimaryKeyMap
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryKeyMap"/> struct with the specified column name and property name.
    /// </summary>
    public PrimaryKeyMap(string ColumnName, string PropertyName)
    {
        this.ColumnName = ColumnName;
        this.PropertyName = PropertyName;
    }

    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string PropertyName { get; }
}
