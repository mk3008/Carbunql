using Carbunql.Definitions;

namespace Carbunql.Annotations;

//public class PropertyColumnDefinition : ColumnDefinition
//{
//    public PropertyColumnDefinition(string schema, string table, string columnName, string columnType, string propertyName) : base(schema, table, columnName, columnType)
//    {
//        PropertyName = propertyName;
//    }
//    public PropertyColumnDefinition(ITable t, string columnName, string columnType, string propertyName) : base(t, columnName, columnType)
//    {
//        PropertyName = propertyName;
//    }

//    public string PropertyName { get; init; }

//    public string RelationColumnType { get; set; } = string.Empty;

//    public string Comment { get; set; } = string.Empty;

//    public SpecialColumn SpecialColumn { get; set; } = SpecialColumn.None;

//    public IEnumerable<ColumnDefinition> GetColumnDefinitions()
//    {
//        yield return this;
//    }
//}
