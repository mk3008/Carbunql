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

	public IEnumerable<SelectQuery> GetSelectQueries()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetSelectQueries())
			{
				yield return item;
			}
		}
	}

	public abstract IDictionary<string, object?> GetParameters();

	public abstract IEnumerable<Token> GetTokens(Token? parent);
}