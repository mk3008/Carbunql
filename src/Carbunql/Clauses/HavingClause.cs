using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class HavingClause : IQueryCommandable
{
	public HavingClause(ValueBase condition)
	{
		Condition = condition;
	}

	public ValueBase Condition { get; init; }

	public IDictionary<string, object?> GetParameters()
	{
		return Condition.GetParameters();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var clause = Token.Reserved(this, parent, "having");
		yield return clause;

		foreach (var item in Condition.GetTokens(clause)) yield return item;
	}
}