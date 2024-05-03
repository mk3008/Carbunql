namespace Carbunql.Annotations;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class ColumnAttribute : Attribute
{
    public ColumnAttribute()
    {
    }

    public ColumnAttribute(string columnType)
    {
        ColumnType = columnType;
    }

    public string ColumnName { get; set; } = string.Empty;

    public string ColumnType { get; set; } = string.Empty;

    public string RelationColumnType { get; set; } = string.Empty;

    public bool IsAutoNumber { get; set; } = false;

    public bool IsNullable { get; set; } = false;

    public string AutoNumberDefinition { get; set; } = string.Empty;

    public string DefaultValue { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public string TimestampCommand { get; set; } = string.Empty;

    public SpecialColumn SpecialColumn { get; set; } = SpecialColumn.None;

    //public PropertyColumnDefinition ToDefinition(ITable t, PropertyInfo prop)
    //{
    //    var columnName = (!string.IsNullOrEmpty(ColumnName)) ? ColumnName : prop.Name.ToSnakeCase();
    //    var columnType = (!string.IsNullOrEmpty(ColumnType)) ? ColumnType : DbmsConfiguration.ToDbType(prop.PropertyType);
    //    var relationColumnType = (!string.IsNullOrEmpty(RelationColumnType)) ? RelationColumnType : DbmsConfiguration.ToDbType(prop.PropertyType);

    //    var c = new PropertyColumnDefinition(t, columnName, columnType, prop.Name)
    //    {
    //        RelationColumnType = relationColumnType,
    //        Comment = Comment,
    //        IsAutoNumber = IsAutoNumber,
    //        SpecialColumn = SpecialColumn,
    //        IsNullable = prop.IsNullable(),
    //    };

    //    if (IsAutoNumber && !string.IsNullOrEmpty(AutoNumberDefinition))
    //    {
    //        c.DefaultValue = ValueParser.Parse(AutoNumberDefinition);
    //    }
    //    if (!string.IsNullOrEmpty(DefaultValue))
    //    {
    //        c.DefaultValue = ValueParser.Parse(DefaultValue);
    //    }
    //    return c;
    //}
}
