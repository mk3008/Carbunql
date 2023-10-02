using Carbunql.Building;
using System.Reflection;

namespace Carbunql.Postgres;

internal static class TypeExtension
{
	internal static string ToTableName(this Type type)
	{
		var atr = type.GetCustomAttribute(typeof(TableDefinitionAttribute)) as TableDefinitionAttribute;
		if (atr == null || string.IsNullOrEmpty(atr.Table))
		{
			return type.Name;
		}
		else
		{
			return atr.Table;
		}
	}
}
