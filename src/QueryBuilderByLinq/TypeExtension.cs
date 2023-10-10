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
}
