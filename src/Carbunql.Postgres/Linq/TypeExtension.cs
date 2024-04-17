using Carbunql.Building;
using System.Reflection;

namespace Carbunql.Postgres.Linq;

internal static class TypeExtension
{
    internal static string ToTableName(this Type type)
    {
        var atr = type.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
        if (atr != null && !string.IsNullOrEmpty(atr.GetTableFullName()))
        {
            return atr.GetTableFullName();
        }
        else
        {
            return type.Name;
        }
    }

    public static bool IsAnonymousType(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
               && type.IsGenericType && type.Name.Contains("AnonymousType", StringComparison.Ordinal)
               && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase)
                   || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
               && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
    }

    internal static bool ToTryDbType(this Type type, out string dbType)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean:
                dbType = "boolean";
                return true;
            case TypeCode.Char:
                dbType = "character";
                return true;
            case TypeCode.SByte:
                dbType = "smallint";
                return true;
            case TypeCode.Byte:
                dbType = "smallint"; // PostgreSQLにはbyteに相当する型がないためsmallintにマッピング
                return true;
            case TypeCode.Int16:
                dbType = "smallint";
                return true;
            case TypeCode.UInt16:
                dbType = "integer"; // PostgreSQLにはushortに相当する型がないためintegerにマッピング
                return true;
            case TypeCode.Int32:
                dbType = "integer";
                return true;
            case TypeCode.UInt32:
                dbType = "bigint"; // PostgreSQLにはuintに相当する型がないためbigintにマッピング
                return true;
            case TypeCode.Int64:
                dbType = "bigint";
                return true;
            case TypeCode.UInt64:
                dbType = "numeric"; // PostgreSQLにはulongに相当する型がないためnumericにマッピング
                return true;
            case TypeCode.Single:
                dbType = "real";
                return true;
            case TypeCode.Double:
                dbType = "double precision";
                return true;
            case TypeCode.Decimal:
                dbType = "numeric";
                return true;
            case TypeCode.DateTime:
                dbType = "timestamp";
                return true;
            case TypeCode.String:
                dbType = "text";
                return true;
            default:
                dbType = string.Empty;
                return false;
        }
    }
}
