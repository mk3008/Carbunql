using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class DropConstraintCommand : IAlterCommand
{
	public DropConstraintCommand(string constraintName)
	{
		ConstraintName = constraintName;
	}

	public string ConstraintName { get; set; }

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
		yield return new Token(this, parent, "constraint", isReserved: true);
		yield return new Token(this, parent, ConstraintName);
	}
}