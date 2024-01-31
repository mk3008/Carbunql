namespace Carbunql.Clauses;

public class MergeWhenNothing : MergeCondition
{
	public MergeWhenNothing()
	{
	}

	public bool IsMatchCondition { get; set; }

	public override IEnumerable<QueryParameter> GetParameters()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetParameters())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		var txt = (IsMatchCondition) ? "when matched" : "when not matched";
		var t = Token.Reserved(this, parent, txt);
		yield return t;
		foreach (var item in GetConditionTokens(t)) yield return item;
		yield return Token.Reserved(this, parent, "then");
		yield return Token.Reserved(this, parent, "do nothing");
	}
}