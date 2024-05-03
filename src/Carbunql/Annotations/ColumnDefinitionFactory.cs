using Carbunql.Analysis.Parser;
using Carbunql.Definitions;
using System.Reflection;

namespace Carbunql.Annotations;

public static class ColumnDefinitionFactory
{
    private static List<Type> LiteralTypes = new List<Type> {
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?),
            typeof(bool),
            typeof(bool?),
            typeof(char),
            typeof(char?),
            typeof(string),
            typeof(DateTime),
            typeof(DateTime?)
    };

    public static IList<ColumnDefinition> Create<T>(ITable t)
    {


        // ・無効属性があるプロパティは除外します。
        // ・プロパティの型がリテラルでない場合は除外します。
        var props = typeof(T).GetProperties()
            .Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreMappingAttribute)))
            .Where(prop => LiteralTypes.Contains(prop.PropertyType))
            .Select(prop => new
            {
                Property = prop,
                Attribute = (ColumnAttribute?)Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute))
            })
            .Select(x => x.Attribute != null
                ? Create<T>(t, x.Property, x.Attribute.ColumnName, x.Attribute.ColumnType, x.Attribute.IsAutoNumber, x.Attribute.IsNullable, x.Attribute.DefaultValue)
                : Create<T>(t, x.Property, string.Empty, string.Empty, null, null, string.Empty)
            );

        return props.ToList();

        //// ・無効属性があるプロパティは除外します。
        //// ・プロパティの型がリテラルでない場合、親になれる場合は含めます
        //var parentprops = typeof(T).GetProperties()
        //    .Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreMappingAttribute)))
        //    .Where(prop => !literalTypes.Contains(prop.PropertyType))
        //    .Where(prop => Attribute.IsDefined(prop, typeof(ParentRelationAttribute))
        //                    || prop.PropertyType.GetInterface(nameof(IEnumerable)) == null)
        //    .Select(prop => new
        //    {
        //        Property = prop,
        //        Attribute = (ParentRelationAttribute?)Attribute.GetCustomAttribute(prop, typeof(ParentRelationAttribute))
        //    });
    }

    private static ColumnDefinition Create<T>(ITable t, PropertyInfo prop, string columnName, string columnType, bool? isAutonumber, bool? isNullable, string defaultValue)
    {
        if (string.IsNullOrEmpty(columnName))
        {
            columnName = DbmsConfiguration.ConvertToDefaultColumnNameLogic(prop.Name);
        }

        if (isAutonumber == null)
        {

            isAutonumber = prop.IsAutoNumber();
        }

        if (string.IsNullOrEmpty(columnType))
        {
            if (isAutonumber.Value)
            {
                columnType = DbmsConfiguration.ToIdentityDbType(prop.PropertyType);
            }
            else
            {
                columnType = DbmsConfiguration.ToDbType(prop.PropertyType);
            }
        }

        if (isNullable == null)
        {
            isNullable = prop.IsDbNullable();
        }

        var def = new ColumnDefinition(t, columnName, ValueParser.Parse(columnType))
        {
            IsAutoNumber = isAutonumber.Value,
            IsNullable = isNullable.Value,
        };

        if (def.IsAutoNumber && AutoNumberDefinitionFactory.TryCreate<T>(out var autoNumberDefinition))
        {
            def.AutoNumberDefinition = autoNumberDefinition;
        }

        if (!string.IsNullOrEmpty(defaultValue))
        {
            def.DefaultValue = ValueParser.Parse(defaultValue);
        }

        return def;
    }




    //private static List<PropertyInfo> GetDefaultColumnProperties<T>()
    //{
    //    var props = typeof(T).GetProperties()
    //                  .Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreMappingAttribute))).ToList();
    //    return props;
    //}

    //private static Dictionary<PropertyInfo, ColumnAttribute> GetColumnProperties<T>()
    //{
    //    var props = typeof(T).GetProperties()
    //                  .Where(prop => Attribute.IsDefined(prop, typeof(ColumnAttribute)))
    //                  .ToDictionary(prop => prop, prop => (ColumnAttribute)Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute))!);
    //    return props;
    //}

    //private static PropertyColumnDefinition CreatePropertyColumnDefinition(ITable t, PropertyInfo prop, ColumnAttribute atr)
    //{
    //    var name = !string.IsNullOrEmpty(atr.ColumnName) ? atr.ColumnName : DbmsConfiguration.ConvertToDefaultColumnNameLogic(prop.Name);
    //    var type = !string.IsNullOrEmpty(atr.ColumnType) ? atr.ColumnType : DbmsConfiguration.ToDbType(prop.PropertyType);
    //    var relationType = (!string.IsNullOrEmpty(atr.RelationColumnType)) ? atr.RelationColumnType : DbmsConfiguration.ToDbType(prop.PropertyType);

    //    var def = new PropertyColumnDefinition(t, name, type, prop.Name)
    //    {
    //        RelationColumnType = relationType,
    //        Comment = atr.Comment,
    //        IsAutoNumber = atr.IsAutoNumber,
    //        SpecialColumn = atr.SpecialColumn,
    //        IsNullable = prop.IsDbNullable(),
    //    };

    //    if (atr.IsAutoNumber && !string.IsNullOrEmpty(atr.AutoNumberDefinition))
    //    {
    //        def.AutoNumberDefinition = ValueParser.Parse(atr.AutoNumberDefinition);
    //    }
    //    if (!string.IsNullOrEmpty(atr.DefaultValue))
    //    {
    //        def.DefaultValue = ValueParser.Parse(atr.DefaultValue);
    //    }

    //    return def;
    //}

    //private static PropertyColumnDefinition CreatePropertyColumnDefinition(ITable t, PropertyInfo prop)
    //{
    //    var atr = (ColumnAttribute?)Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute));
    //    if (atr != null)
    //    {
    //        return CreatePropertyColumnDefinition(t, prop, atr);
    //    }

    //    var name = DbmsConfiguration.ConvertToDefaultColumnNameLogic(prop.Name);
    //    var type = DbmsConfiguration.ToDbType(prop.PropertyType);
    //    var relationType = DbmsConfiguration.ToDbType(prop.PropertyType);

    //    var def = new PropertyColumnDefinition(t, name, type, prop.Name)
    //    {
    //        RelationColumnType = relationType,
    //        Comment = $"{prop.Name} property mapping column",
    //        IsNullable = prop.IsDbNullable(),
    //    };

    //    if (DbmsConfiguration.IsPrimaryKeyColumnLogic(t.Table, name))
    //    {
    //        def.ColumnType = ValueParser.Parse(DbmsConfiguration.ToIdentityDbType(prop.PropertyType));
    //        def.IsAutoNumber = true;
    //        def.AutoNumberDefinition = ValueParser.Parse(DbmsConfiguration.GetDefaultAutoNumberDefinitionLogic(t.Table, name));
    //    }

    //    return def;
    //}
}
