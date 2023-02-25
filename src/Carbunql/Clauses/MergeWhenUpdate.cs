using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class MergeWhenUpdate : MergeCondition
{
	public MergeWhenUpdate(MergeUpdateQuery query)
	{
		Query = query;
	}

	public MergeUpdateQuery Query { get; init; }

	public override IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Query.GetParameters());
		prm = prm.Merge(Condition?.GetParameters());
		return prm;
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		var t = Token.Reserved(this, parent, "when matched");
		yield return t;
		foreach (var item in GetConditionTokens(t)) yield return item;
		yield return Token.Reserved(this, parent, "then");
		foreach (var item in Query.GetTokens(t)) yield return item;
	}
}