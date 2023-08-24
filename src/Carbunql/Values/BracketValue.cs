using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class BracketValue : ValueBase
{
	public BracketValue()
	{
		Inner = null!;
	}

	public BracketValue(ValueBase inner)
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
		if (Inner == null) yield break;

		var bracket = Token.ExpressionBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Inner.GetTokens(bracket)) yield return item;
		yield return Token.ExpressionBracketEnd(this, parent);
	}
}