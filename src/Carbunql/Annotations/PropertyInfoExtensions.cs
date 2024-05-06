using Carbunql.Values;
using System.Reflection;

namespace Carbunql.Annotations;

/// <summary>
/// Extension methods for PropertyInfo class.
/// </summary>
public static class PropertyInfoExtension
{
    /// <summary>
    /// Converts the property value to a ParameterValue with the specified placeholder identifier.
    /// </summary>
    /// <typeparam name="T">The type of the instance containing the property.</typeparam>
    /// <param name="prop">The property to convert.</param>
    /// <param name="instance">The instance containing the property.</param>
    /// <param name="placeholderIdentifier">The placeholder identifier to prepend to the key.</param>
    /// <returns>A ParameterValue containing the property value.</returns>
    public static ParameterValue ToParameterValue<T>(this PropertyInfo prop, T instance, string placeholderIdentifier)
    {
        var value = prop.GetValue(instance);
        var key = placeholderIdentifier + prop.Name;
        return new ParameterValue(key, value);
    }

    /// <summary>
    /// Converts the property to a ParameterValue with a null value and the specified placeholder identifier.
    /// </summary>
    /// <param name="prop">The property to convert.</param>
    /// <param name="placeholderIdentifier">The placeholder identifier to prepend to the key.</param>
    /// <returns>A ParameterValue with a null value.</returns>
    public static ParameterValue ToParameterNullValue(this PropertyInfo prop, string placeholderIdentifier)
    {
        var key = placeholderIdentifier + prop.Name;
        return new ParameterValue(key, null);
    }

    /// <summary>
    /// Indicates whether the specified property should be nullable when mapping to a database column.
    /// </summary>
    /// <param name="prop">The property to check.</param>
    /// <returns><c>true</c> if the specified property should be nullable; otherwise, <c>false</c>.</returns>
    public static bool IsDbNullable(this PropertyInfo prop)
    {
        var propertyType = prop.PropertyType;

        // Treat string type as non-nullable in the context of a database.
        // NOTE: Even Nullable<string> is considered as non-nullable.
        // Reason: Allowing NULLs should be minimized as they are hard to handle,
        // and empty strings can be used as substitutes, hence this decision.
        if (propertyType == typeof(string)) return false;

        // Treat reference types as nullable.
        if (!propertyType.IsValueType) return true;

        // Check if the type is a nullable value type.
        if (Nullable.GetUnderlyingType(propertyType) != null) return true;

        // Otherwise, treat as non-nullable.
        return false;
    }

    /// <summary>
    /// Indicates whether the specified property is an auto-incrementing column.
    /// </summary>
    /// <param name="prop">The property to check.</param>
    /// <returns><c>true</c> if the specified property is an auto-incrementing column; otherwise, <c>false</c>.</returns>
    public static bool IsAutoNumber(this PropertyInfo prop)
    {
        // If the property type is not a numeric type, return false.
        if (!IsNumericType(prop.PropertyType)) return false;

        // Get the TableAttribute of the class to which the property belongs.
        var attribute = prop.DeclaringType?.GetCustomAttribute<TableAttribute>();

        if (attribute != null)
        {
            if (attribute.PrimaryKeyProperties.Count() == 1 && attribute.PrimaryKeyProperties.First() == prop.Name) return true;
            return false;
        }

        // If TableAttribute is not found, infer auto-incrementing from default naming conventions.
        var tableName = DbmsConfiguration.ConvertToDefaultTableNameLogic(prop.DeclaringType!);
        var columnName = DbmsConfiguration.ConvertToDefaultColumnNameLogic(prop.Name);
        if (DbmsConfiguration.IsPrimaryKeyColumnLogic(tableName, columnName))
        {
            return true;
        }
        return false;
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) || type == typeof(decimal);
    }
}
