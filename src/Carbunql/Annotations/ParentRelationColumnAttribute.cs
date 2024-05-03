using Carbunql.Building;
using Carbunql.Definitions;
using System.Reflection;

namespace Carbunql.Annotations;

//[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
//public class ParentRelationColumnAttribute : Attribute
//{
//    public ParentRelationColumnAttribute(string columnType, string parentIdentifer)
//    {
//        ColumnType = columnType;
//        ParentProperty = parentIdentifer;
//    }

//    public ParentRelationColumnAttribute(string columnName, string columnType, string parentIdentifer)
//    {
//        ColumnName = columnName;
//        ColumnType = columnType;
//        ParentProperty = parentIdentifer;
//    }

//    public string ColumnName { get; set; } = string.Empty;

//    public string ParentProperty { get; set; }

//    public string ColumnType { get; set; }

//    public string Comment { get; set; } = string.Empty;

//    public ParentRelationColumnDefinition ToDefinition(ITable t, PropertyInfo prop)
//    {
//        var columnName = !string.IsNullOrEmpty(ColumnName) ? ColumnName : prop.Name.ToSnakeCase() + "_id";

//        var d = new ParentRelationColumnDefinition(t, columnName, ColumnType, prop.Name, ParentProperty)
//        {
//            Comment = Comment,
//            IsNullable = prop.IsDbNullable(),
//            IsAutoNumber = false,
//            //RelationColumnType = ColumnType,
//            SpecialColumn = SpecialColumn.ParentRelation
//        };
//        return d;
//    }
//}

