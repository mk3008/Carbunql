using Carbunql.Clauses;

namespace Carbunql.Values;

public class LikeExpression : ValueBase
{
	public LikeExpression(ValueBase value, ValueBase argument, bool isNegative = false)
	{
		Value = value;
		Argument = argument;
		IsNegative = isNegative;
	}

	public ValueBase Value { get; init; }

	public ValueBase Argument { get; init; }

	public bool IsNegative { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Argument.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		if (IsNegative) yield return Token.Reserved(this, parent, "not");

		yield return Token.Reserved(this, parent, "like");
		foreach (var item in Argument.GetTokens(parent)) yield return item;
	}
}