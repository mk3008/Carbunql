using Carbunql.Clauses;

namespace Carbunql.Values;

public class InExpression : ValueBase
{
	public InExpression(ValueBase value, ValueBase argument)
	{
		Value = value;
		Argument = argument;
		IsNegative = false;
	}

	public InExpression(ValueBase value, ValueBase argument, bool isNegative)
	{
		Value = value;
		Argument = argument;
		IsNegative = isNegative;
	}

	public ValueBase Value { get; init; }

	public ValueBase Argument { get; init; }

	public bool IsNegative { get; init; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		if (IsNegative) yield return Token.Reserved(this, parent, "not");

		yield return Token.Reserved(this, parent, "in");
		foreach (var item in Argument.GetTokens(parent)) yield return item;
	}
}