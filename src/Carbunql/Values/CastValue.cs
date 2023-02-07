using Carbunql.Clauses;

namespace Carbunql.Values;

public class CastValue : ValueBase
{
	public CastValue(ValueBase inner, string symbol, ValueBase type)
	{
		Inner = inner;
		Symbol = symbol;
		Type = type;
	}

	public ValueBase Inner { get; init; }

	public string Symbol { get; init; }

	public ValueBase Type { get; init; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Inner.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, Symbol);
		foreach (var item in Type.GetTokens(parent)) yield return item;
	}
}