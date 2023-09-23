namespace Carbunql.Building;

/// <summary>
/// When used with Expression, it is treated as an expression.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class RecordDefinitionAttribute : Attribute
{
	public string Schema { get; set; } = string.Empty;
	public string Table { get; set; } = string.Empty;
}