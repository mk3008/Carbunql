using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Carbunql.Annotations;

//public class ParentRelationDefinition : ITableDefinition
//{
//    public ParentRelationDefinition(ITable t, string propertyName, Type propertyType, bool isNullable)
//    {
//        Schema = t.Schema;
//        Table = t.Table;
//        PropertyName = propertyName;
//        PropertyType = propertyType;
//        IsNullable = isNullable;
//    }

//    public ParentRelationDefinition(ITable t, PropertyInfo prop)
//    {
//        Schema = t.Schema;
//        Table = t.Table;
//        PropertyName = prop.Name;
//        PropertyType = prop.PropertyType;
//        IsNullable = prop.IsDbNullable();
//    }

//    public string PropertyName { get; init; }

//    public Type PropertyType { get; init; }

//    public bool IsNullable { get; init; }

//    public List<ParentRelationColumnDefinition> Relations { get; } = new();

//    public string ColumnName => string.Empty;

//    public string Schema { get; init; }

//    public string Table { get; init; }

//    //public IEnumerable<ColumnDefinition> GetColumnDefinitions()
//    //{
//    //    foreach (var relation in Relations)
//    //    {
//    //        foreach (var item in relation.GetColumnDefinitions())
//    //        {
//    //            item.SpecialColumn = SpecialColumn.ParentRelation;
//    //            yield return item;
//    //        }
//    //    }
//    //}

//    public IEnumerable<CommonTable> GetCommonTables()
//    {
//        yield break;
//    }

//    public IEnumerable<SelectQuery> GetInternalQueries()
//    {
//        yield break;
//    }

//    public IEnumerable<QueryParameter> GetParameters()
//    {
//        yield break;
//    }

//    public IEnumerable<PhysicalTable> GetPhysicalTables()
//    {
//        yield break;
//    }

//    public IEnumerable<Token> GetTokens(Token? parent)
//    {
//        yield break;
//    }

//    public bool TryDisasseble([MaybeNullWhen(false)] out IConstraint constraint)
//    {
//        constraint = default;
//        return false;
//    }

//    public bool TrySet(TableDefinitionClause clause)
//    {
//        return false;
//    }

//    //public IEnumerable<string> GetCreateTableCommandTexts()
//    //{
//    //    foreach (var def in GetColumnDefinitions())
//    //    {
//    //        foreach (var item in def.GetCreateTableCommandTexts())
//    //        {
//    //            yield return item;
//    //        }
//    //    }
//    //}
//}
