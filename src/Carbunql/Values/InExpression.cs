using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class InExpression : ValueBase
{
	public InExpression()
	{
		Value = null!;
		Argument = null!;
		IsNegative = false;
	}

	public InExpression(ValueBase value, ValueBase argument)
	{
		Value = value;
		if (argument is BracketValue || argument is QueryContainer)
		{
			Argument = argument;
		}
		else
		{
			Argument = new BracketValue(argument);
		}
		IsNegative = false;
	}

	public InExpression(ValueBase value, ValueBase argument, bool isNegative)
	{
		Value = value;
		if (argument is BracketValue || argument is QueryContainer)
		{
			Argument = argument;
		}
		else
		{
			Argument = new BracketValue(argument);
		}
		IsNegative = isNegative;
	}

	[Key(1)]
	public ValueBase Value { get; init; }

	[Key(2)]
	public ValueBase Argument { get; init; }

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

	[Key(3)]
	public bool IsNegative { get; init; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		if (IsNegative) yield return Token.Reserved(this, parent, "not");

		yield return Token.Reserved(this, parent, "in");
		foreach (var item in Argument.GetTokens(parent)) yield return item;
	}
}