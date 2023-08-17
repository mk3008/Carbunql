using Carbunql.Clauses;

namespace Carbunql.Values;

public class FromArgument : ValueBase
{
	public FromArgument(ValueBase unit, ValueBase value)
	{
		Unit = unit;
		Value = value;
	}

	public ValueBase Unit { get; init; }

	public ValueBase Value { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Unit.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Unit.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "from");
		foreach (var item in Value.GetTokens(parent)) yield return item;
	}
}