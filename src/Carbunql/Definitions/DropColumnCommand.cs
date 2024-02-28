using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class DropColumnCommand : IAlterCommand
{
	public DropColumnCommand(string columnName)
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
		yield return new Token(this, parent, "drop", isReserved: true);
		yield return new Token(this, parent, "column", isReserved: true);
		yield return new Token(this, parent, ColumnName);
	}
}