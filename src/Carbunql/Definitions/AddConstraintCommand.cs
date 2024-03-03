using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Definitions;

public class AddConstraintCommand : IAlterCommand
{
	public AddConstraintCommand(IConstraint constraint)
	{
		Constraint = constraint;
	}

	public IConstraint Constraint { get; set; }

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
		yield return new Token(this, parent, "add", isReserved: true);
		foreach (var item in Constraint.GetTokens(parent))
		{
			yield return item;
		}
	}

	public bool TryIntegrate(TableDefinitionClause clause)
	{
		return Constraint.TryIntegrate(clause);
	}
}