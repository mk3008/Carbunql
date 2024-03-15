namespace Carbunql.Definitions;

public interface ITable
{
	string? Schema { get; }

	string Table { get; }
}

public static class TableExtension
{
	static public string GetTableFullName(this ITable t)
	{
		return string.IsNullOrEmpty(t.Schema) ? t.Table : t.Schema + "." + t.Table;
	}
}