using Carbunql.Building;
using System.Reflection;

namespace QueryBuilderByLinq;

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

}
