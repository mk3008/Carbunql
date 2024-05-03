using Carbunql.Values;
using System.Reflection;

namespace Carbunql.Annotations;

public static class PropertyInfoExtenstion
{
    public static ParameterValue ToParameterValue<T>(this PropertyInfo prop, T instance, string placeholderIdentifer)
    {
        var value = prop.GetValue(instance);
        var key = placeholderIdentifer + prop.Name;
        return new ParameterValue(key, value);
    }

    public static ParameterValue ToParameterNullValue(this PropertyInfo prop, string placeholderIdentifer)
    {
        var key = placeholderIdentifer + prop.Name;
        return new ParameterValue(key, null);
    }

    /// <summary>
    /// Indicates whether the specified property should be nullable when mapping to a database column.
    /// </summary>
    /// <param name="prop">The property to check.</param>
    /// <returns><c>true</c> if the specified property should be nullable; otherwise, <c>false</c>.</returns>
    public static bool IsDbNullable(this PropertyInfo prop)
    {
        var proptype = prop.PropertyType;

        // Treat string type as non-nullable in the context of a database.
        // NOTE:
        // Even Nullable<string> is considered as non-nullable.
        // Reason:
        // Allowing NULLs should be minimized as they are hard to handle,
        // and empty strings can be used as substitutes, hence this decision.
        if (prop.PropertyType == typeof(string)) return false;

        // Treat reference types as nullable.
        if (!proptype.IsValueType) return true;

        // Check if the type is a nullable value type.
        if (Nullable.GetUnderlyingType(proptype) != null) return true;

        // Otherwise, treat as non-nullable.
        return false;
    }

    public static bool IsAutoNumber(this PropertyInfo prop)
    {
        // プロパティの型が数値型出ない場合、false
        if (!IsNumericType(prop.PropertyType)) return false;

        // プロパティが属しているクラスのTableAttributeを取得する
        var atr = prop.DeclaringType?.GetCustomAttribute<TableAttribute>();

        if (atr != null)
        {
            if (atr.PrimaryKeyProperties.Count() == 1 && atr.PrimaryKeyProperties.First() == prop.Name) return true;
            return false;
        }

        var table = DbmsConfiguration.ConvertToDefaultTableNameLogic(prop.DeclaringType!);
        var column = DbmsConfiguration.ConvertToDefaultColumnNameLogic(prop.Name);
        if (DbmsConfiguration.IsPrimaryKeyColumnLogic(table, column))
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
