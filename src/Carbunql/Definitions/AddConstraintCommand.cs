using Carbunql.Clauses;
using Carbunql.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Definitions;

public class AddConstraintCommand : IAlterCommand
{
	public AddConstraintCommand(IConstraint constraint)
	{
		Constraint = constraint;
	}

	public IConstraint Constraint { get; set; }

	public string? Schema => Constraint.Schema;

	public string Table => Constraint.Table;

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

	public bool TrySet(TableDefinitionClause clause)
	{
		return Constraint.TrySet(clause);
	}

	//public bool TryToIndex([MaybeNullWhen(false)] out CreateIndexQuery query)
	//{
	//	return Constraint.TryToIndex(out query);
	//}
}