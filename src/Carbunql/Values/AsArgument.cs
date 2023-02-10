using Carbunql.Clauses;

namespace Carbunql.Values;

public class AsArgument : ValueBase
{
	public AsArgument(ValueBase value, ValueBase type)
	{
		Value = value;
		Type = type;
	}

	public ValueBase Value { get; init; }

	public ValueBase Type { get; init; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "as");
		foreach (var item in Type.GetTokens(parent)) yield return item;
	}
}