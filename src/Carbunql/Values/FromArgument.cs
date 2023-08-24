﻿using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class FromArgument : ValueBase
{
	public FromArgument()
	{
		Unit = null!;
		Value = null!;
	}

	public FromArgument(ValueBase unit, ValueBase value)
	{
		Unit = unit;
		Value = value;
	}

	[Key(1)]
	public ValueBase Unit { get; init; }

	[Key(2)]
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