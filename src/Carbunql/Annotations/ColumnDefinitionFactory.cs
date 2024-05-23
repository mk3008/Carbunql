using Carbunql.Analysis.Parser;
using Carbunql.Definitions;
using System.Reflection;

namespace Carbunql.Annotations;

/// <summary>
/// Factory class for creating column definitions.
/// </summary>
public static class ColumnDefinitionFactory
{
    /// <summary>
    /// Creates column definitions for the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of class for which to create the column definitions.</typeparam>
    /// <param name="t">The table associated with the column definitions.</param>
    /// <returns>A list of generated column definitions.</returns>
    public static List<ColumnDefinition> Create<T>(ITable t)
    {
        var lst = new List<ColumnDefinition>();
        lst.AddRange(CreateFromLiteralProperties<T>(t));
        lst.AddRange(CreateFromRelationPropeties<T>(t));
        return lst;
    }

    private static List<ColumnDefinition> CreateFromLiteralProperties<T>(ITable t)
    {
        // Exclude properties with invalid attributes.
        // Exclude properties with non-literal types.
        var props = PropertySelector.SelectLiteralProperties<T>()
            .Select(prop => CreateFromLiteralProperty(typeof(T), t, prop));

        return props.ToList();
    }

    internal static ColumnDefinition CreateFromLiteralProperty(Type type, ITable t, PropertyInfo prop)
    {
        var attribute = (ColumnAttribute?)Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute));

        if (attribute == null)
        {
            return Create(type, t, prop, string.Empty, string.Empty, string.Empty, null, null, string.Empty);

        }
        else
        {
            return Create(type, t, prop, attribute.ColumnName, attribute.ColumnType, attribute.RelationColumnType, attribute.IsAutoNumber, attribute.IsNullable, attribute.DefaultValue);
        }
    }

    /// <summary>
    /// Generates database column definitions from properties of related types.
    /// </summary>
    /// <typeparam name="T">The type to generate column definitions for.</typeparam>
    /// <param name="t">The table associated with the column definitions.</param>
    /// <returns>A list of generated column definitions.</returns>
    public static List<ColumnDefinition> CreateFromRelationPropeties<T>(ITable t)
    {
        var parentprops = PropertySelector.SelectParentProperties<T>()
            .SelectMany(prop => CreateAsParentProperties(t, prop));

        return parentprops.ToList();
    }

    internal static List<ColumnDefinition> CreateAsParentProperties(ITable t, PropertyInfo prop)
    {
        var parentType = prop.PropertyType;
        var parentclause = TableDefinitionClauseFactory.Create(parentType);
        var parentPkeyMaps = PrimaryKeyConstraintFactory.Create(parentType).PrimaryKeyMaps;
        if (!parentPkeyMaps.Any()) throw new InvalidProgramException();

        var attributes = (IEnumerable<ParentRelationColumnAttribute>)Attribute.GetCustomAttributes(prop, typeof(ParentRelationColumnAttribute));

        return parentPkeyMaps.Select(map => Create(parentType, t, map)).ToList();
    }

    private static ColumnDefinition Create(Type parentType, ITable t, PrimaryKeyMap map)
    {
        var prop = parentType.GetProperty(map.PropertyName)!;
        var parentColumn = CreateFromLiteralProperty(parentType, t, prop);
        return new ColumnDefinition(t, map.ColumnName, parentColumn.RelationColumnType);
    }

    private static ColumnDefinition Create<T>(ITable t, PropertyInfo prop, string columnName, string columnType, string relationColumnType, bool? isAutonumber, bool? isNullable, string defaultValue)
    {
        return Create(typeof(T), t, prop, columnName, columnType, relationColumnType, isAutonumber, isNullable, defaultValue);
    }

    private static ColumnDefinition Create(Type type, ITable t, PropertyInfo prop, string columnName, string columnType, string relationColumnType, bool? isAutonumber, bool? isNullable, string defaultValue)
    {
        if (string.IsNullOrEmpty(columnName))
        {
            columnName = DbmsConfiguration.ConvertToDefaultColumnNameLogic(prop.Name);
        }

        if (isAutonumber == null)
        {
            isAutonumber = prop.IsAutoNumber();
        }

        if (string.IsNullOrEmpty(relationColumnType))
        {
            relationColumnType = DbmsConfiguration.ToDbType(prop.PropertyType);
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
            RelationColumnType = ValueParser.Parse(relationColumnType),
            IsAutoNumber = isAutonumber.Value,
            IsNullable = isNullable.Value,
        };

        if (def.IsAutoNumber && AutoNumberDefinitionFactory.TryCreate(type, out var autoNumberDefinition))
        {
            def.AutoNumberDefinition = autoNumberDefinition;
        }

        if (!string.IsNullOrEmpty(defaultValue))
        {
            def.DefaultValue = ValueParser.Parse(defaultValue);
        }

        return def;
    }
}
