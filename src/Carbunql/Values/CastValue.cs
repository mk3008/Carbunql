using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class CastValue : ValueBase
{
	public CastValue()
	{
		Inner = null!;
		Symbol = null!;
		Type = null!;
	}

	public CastValue(ValueBase inner, string symbol, ValueBase type)
	{
		Inner = inner;
		Symbol = symbol;
		Type = type;
	}

	[Key(1)]
	public ValueBase Inner { get; init; }

	[Key(2)]
	public string Symbol { get; init; }

	[Key(3)]
	public ValueBase Type { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Inner.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Type.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Inner.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, Symbol);
		foreach (var item in Type.GetTokens(parent)) yield return item;
	}
}