using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class ChangeColumnTypeCommand : IAlterCommand
{
	public ChangeColumnTypeCommand(string columnName, ValueBase columnType)
	{
		ColumnName = columnName;
		ColumnType = columnType;
	}

	public string ColumnName { get; set; }

	public ValueBase ColumnType { get; set; }

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
		yield return new Token(this, parent, "type", isReserved: true);
		foreach (var item in ColumnType.GetTokens(parent))
		{
			yield return item;
		}
	}

	public bool TryIntegrate(TableDefinitionClause clause)
	{
		var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();

		c.ColumnType = ColumnType;
		return true;
	}
}