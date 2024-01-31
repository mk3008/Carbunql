using Carbunql.Tables;

namespace Carbunql.Clauses;

public abstract class MergeCondition : IQueryCommandable
{
	public ValueBase? Condition { get; set; }

	public IEnumerable<Token> GetConditionTokens(Token? parent)
	{
		if (Condition == null) yield break;
		yield return Token.Reserved(this, parent, "and");
		foreach (var item in Condition.GetTokens(parent)) yield return item;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public abstract IEnumerable<QueryParameter> GetParameters();

	public abstract IEnumerable<Token> GetTokens(Token? parent);
}