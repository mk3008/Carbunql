using Carbunql.Clauses;

namespace Carbunql.Values;

public class BracketValue : ValueBase
{
	public BracketValue(ValueBase inner)
	{
		Inner = inner;
	}

	public ValueBase Inner { get; init; }

	internal override IEnumerable<SelectQuery> GetSelectQueriesCore()
	{
		foreach (var item in Inner.GetSelectQueries())
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