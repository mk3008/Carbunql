using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class BetweenExpression : ValueBase
{
	public BetweenExpression()
	{
		Value = null!;
		Start = null!;
		End = null!;
		IsNegative = false;
	}

	public BetweenExpression(ValueBase value, ValueBase start, ValueBase end, bool isNegative)
	{
		Value = value;
		Start = start;
		End = end;
		IsNegative = isNegative;
	}

	public ValueBase Value { get; init; }

	public ValueBase Start { get; init; }

	public ValueBase End { get; init; }

	public bool IsNegative { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Start.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in End.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		if (IsNegative) yield return Token.Reserved(this, parent, "not");

		yield return Token.Reserved(this, parent, "between");
		foreach (var item in Start.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "and");
		foreach (var item in End.GetTokens(parent)) yield return item;
	}
}