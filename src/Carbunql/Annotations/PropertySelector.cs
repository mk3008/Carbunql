using System.Collections;
using System.Data;
using System.Reflection;

namespace Carbunql.Annotations;

/// <summary>
/// Utility class for selecting properties of a type based on certain criteria.
/// </summary>
public class PropertySelector
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

    /// <summary>
    /// Selects properties of literal types for the specified type T.
    /// </summary>
    /// <typeparam name="T">The type whose properties are selected.</typeparam>
    /// <returns>An enumerable of PropertyInfo representing the selected properties.</returns>
    public static IEnumerable<PropertyInfo> SelectLiteralProperties<T>()
    {
        // Exclude properties with invalid attributes.
        // Exclude properties with non-literal types.
        var props = SelectProperties<T>()
            .Where(prop => LiteralTypes.Contains(prop.PropertyType));

        return props;
    }

    /// <summary>
    /// Selects parent properties for the specified type T.
    /// </summary>
    /// <typeparam name="T">The type whose properties are selected.</typeparam>
    /// <returns>An enumerable of PropertyInfo representing the selected properties.</returns>
    public static IEnumerable<PropertyInfo> SelectParentProperties<T>()
    {
        // Exclude properties with invalid attributes.
        // Include properties whose type is not literal and can be a parent.
        var props = SelectProperties<T>()
            .Where(prop => !LiteralTypes.Contains(prop.PropertyType))
            .Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreMappingAttribute)))
            .Where(prop => !typeof(IDataSet).IsAssignableFrom(prop.PropertyType))
            .Where(prop => Attribute.IsDefined(prop, typeof(ParentRelationColumnAttribute))
                            || prop.PropertyType.GetInterface(nameof(IEnumerable)) == null);

        return props;
    }

    /// <summary>
    /// Selects properties for the specified type T.
    /// </summary>
    /// <typeparam name="T">The type whose properties are selected.</typeparam>
    /// <returns>An enumerable of PropertyInfo representing the selected properties.</returns>
    private static IEnumerable<PropertyInfo> SelectProperties<T>()
    {
        // Exclude properties with invalid attributes.
        var props = typeof(T).GetProperties()
            .Where(prop => !Attribute.IsDefined(prop, typeof(IgnoreMappingAttribute)));

        return props;
    }
}
