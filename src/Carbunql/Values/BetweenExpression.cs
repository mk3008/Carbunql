using Carbunql.Clauses;

namespace Carbunql.Values;

public class BetweenExpression : ValueBase
{
	public BetweenExpression(ValueBase value, ValueBase start, ValueBase end, bool isNegative)
	{
		Value = value;
		Start = start;
		End = end;
		IsNegative = isNegative;
	}

	public ValueBase Value { get; init; }

	public ValueBase Start { get; init; }

	public ValueBase End { get; init; }

	public bool IsNegative { get; init; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		if (IsNegative) yield return Token.Reserved(this, parent, "not");

		yield return Token.Reserved(this, parent, "between");
		foreach (var item in Start.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "and");
		foreach (var item in End.GetTokens(parent)) yield return item;
	}
}