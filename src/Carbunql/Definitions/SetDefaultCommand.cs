using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Definitions;

public class SetDefaultCommand : IAlterCommand
{
	public SetDefaultCommand(string columnName, string defaultValue)
	{
		ColumnName = columnName;
		DefaultValue = new LiteralValue(defaultValue);
	}

	public SetDefaultCommand(string columnName, ValueBase defaultValue)
	{
		ColumnName = columnName;
		DefaultValue = defaultValue;
	}

	public string ColumnName { get; set; }

	public ValueBase DefaultValue { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return new Token(this, parent, "alter column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
		yield return new Token(this, parent, "set", isReserved: true);
		yield return new Token(this, parent, "default", isReserved: true);
		foreach (var item in DefaultValue.GetTokens(parent))
		{
			yield return item;
		}
	}

	public bool TryIntegrate(TableDefinitionClause clause)
	{
		var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();
		c.DefaultValue = DefaultValue;
		return true;
	}
}