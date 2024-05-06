namespace Carbunql.Annotations;

/// <summary>
/// Attribute used to define metadata for a column in a database table.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class ColumnAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the ColumnAttribute class.
    /// </summary>
    public ColumnAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ColumnAttribute class with the specified column type.
    /// </summary>
    /// <param name="columnType">The type of the column.</param>
    public ColumnAttribute(string columnType)
    {
        ColumnType = columnType;
    }

    /// <summary>
    /// Gets or sets the name of the column.
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the column.
    /// </summary>
    public string ColumnType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the related column, if any.
    /// </summary>
    public string RelationColumnType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the column is auto-incremented.
    /// </summary>
    public bool IsAutoNumber { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the column allows null values.
    /// </summary>
    public bool IsNullable { get; set; } = false;

    /// <summary>
    /// Gets or sets the definition for the auto-incremented column.
    /// </summary>
    public string AutoNumberDefinition { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default value for the column.
    /// </summary>
    public string DefaultValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the comment associated with the column.
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command to retrieve a timestamp value.
    /// </summary>
    public string TimestampCommand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the special type of the column, if any.
    /// </summary>
    public SpecialColumn SpecialColumn { get; set; } = SpecialColumn.None;
}
