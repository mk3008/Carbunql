using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class NegativeValue : ValueBase
{
	public NegativeValue()
	{
		Inner = null!;
	}

	public NegativeValue(ValueBase inner)
	{
		Inner = inner;
	}

	[Key(1)]
	public ValueBase Inner { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Inner.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, "not");
		foreach (var item in Inner.GetTokens(parent)) yield return item;
	}
}