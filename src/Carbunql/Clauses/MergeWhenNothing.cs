using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class MergeWhenNothing : MergeCondition
{
	public MergeWhenNothing()
	{
	}

	public bool IsMatchCondition { get; set; }

	public override IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Condition?.GetParameters());
		return prm;
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		var txt = (IsMatchCondition) ? "matched" : "not matched";
		var t = Token.Reserved(this, parent, txt);
		yield return t;
		foreach (var item in GetConditionTokens(t)) yield return item;
		yield return Token.Reserved(this, parent, "then");
		yield return Token.Reserved(this, parent, "do nothing");
	}
}