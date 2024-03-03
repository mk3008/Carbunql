using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class SetNotNullCommand : IAlterCommand
{
	public SetNotNullCommand(string columnName)
	{
		ColumnName = columnName;
	}

	public string ColumnName { get; set; }

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
		yield return new Token(this, parent, "not null", isReserved: true);
	}

	public bool TryIntegrate(TableDefinitionClause clause)
	{
		var c = clause.OfType<ColumnDefinition>().Where(x => x.ColumnName == ColumnName).First();
		c.IsNullable = false;
		return true;
	}
}