using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class MergeWhenDelete : MergeCondition
{
	public MergeWhenDelete()
	{
	}

	public override IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Condition?.GetParameters());
		return prm;
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		var t = Token.Reserved(this, parent, "when matched");
		yield return t;
		foreach (var item in GetConditionTokens(t)) yield return item;
		yield return Token.Reserved(this, parent, "then");
		yield return Token.Reserved(this, parent, "delete");
	}
}