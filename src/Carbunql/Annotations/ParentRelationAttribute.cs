using Carbunql.Definitions;
using System.Reflection;

namespace Carbunql.Annotations;

//[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
//public class ParentRelationAttribute : Attribute
//{
//    public IEnumerable<ITableDefinition> ToDefinitions(ITable t, PropertyInfo prop)
//    {
//        var def = ToParentRealationDefinition(t, prop);
//        yield return def;

//        foreach (var item in def.Relations)
//        {
//            yield return item;
//        }
//    }

//    private ParentRelationDefinition ToParentRealationDefinition(ITable t, PropertyInfo prop)
//    {
//        var def = new ParentRelationDefinition(t, prop);

//        var columns = prop.GetCustomAttributes<ParentRelationColumnAttribute>().ToList();
//        if (columns.Count == 0) throw new InvalidProgramException();

//        foreach (var column in columns)
//        {
//            def.Relations.Add(column.ToDefinition(t, prop));
//        }

//        return def;
//    }

//    //public static ParentRelationAttribute CreateDefault()
//    //{
//    //    var attr = new ParentRelationAttribute();
//    //    return attr;
//    //}
//}
