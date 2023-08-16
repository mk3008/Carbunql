using Carbunql.Clauses;

namespace Carbunql.Values;

public class NegativeValue : ValueBase
{
	public NegativeValue(ValueBase inner)
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
		yield return Token.Reserved(this, parent, "not");
		foreach (var item in Inner.GetTokens(parent)) yield return item;
	}
}