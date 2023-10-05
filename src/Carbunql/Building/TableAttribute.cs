namespace Carbunql.Building;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TableAttribute : System.ComponentModel.DataAnnotations.Schema.TableAttribute
{
	public TableAttribute(string table) : base(table) { }

	public string GetTableFullName()
	{
		if (string.IsNullOrEmpty(Schema)) return Name;
		return Schema + "." + Name;
	}
}