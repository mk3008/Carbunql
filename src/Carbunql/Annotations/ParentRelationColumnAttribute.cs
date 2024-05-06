namespace Carbunql.Annotations;

/// <summary>
/// Attribute for specifying parent relation columns.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public class ParentRelationColumnAttribute : Attribute
{
    public ParentRelationColumnAttribute()
    {
    }

    public ParentRelationColumnAttribute(string parentProperty)
    {
        ParentProperty = parentProperty;
    }

    /// <summary>
    /// Specify when there are multiple relations to the same table or when column names differ between the source and target of the join.
    /// It is similar to ColumnName, but specify Prefix if only a prefix can correspond.
    /// If a prefix cannot express it, specify ColumnName.
    /// </summary>
    public string Prefix { get; set; } = string.Empty;

    /// <summary>
    /// Specify when there are multiple relations to the same table or when column names differ between the source and target of the join.
    /// It is similar to ColumnName, but specify Prefix if only a prefix can correspond.
    /// If a prefix cannot express it, specify ColumnName.
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Specifies the name of the property that corresponds to the primary key of the target type.
    /// </summary>
    public string ParentProperty { get; set; } = string.Empty;

    /// <summary>
    /// Specifies the type of the column.
    /// </summary>
    public string ColumnType { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;
}
