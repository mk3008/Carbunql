using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class RenameTableCommand : IAlterCommand
{
	public RenameTableCommand(string newTableName)
	{
		NewTableName = newTableName;
	}

	public string NewTableName { get; set; }

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
		yield return new Token(this, parent, "rename", isReserved: true);
		yield return new Token(this, parent, "to", isReserved: true);
		yield return new Token(this, parent, NewTableName);
	}
}