namespace Carbunql.Building;

/// <summary>
/// When used with Expression, it is treated as an expression.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TableDefinitionAttribute : Attribute
{
	public TableDefinitionAttribute()
	{
	}

	public TableDefinitionAttribute(string table)
	{
		Table = table;
	}

	public string Table { get; set; } = string.Empty;
}