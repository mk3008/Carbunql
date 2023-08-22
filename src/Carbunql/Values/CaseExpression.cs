﻿using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class CaseExpression : ValueBase
{
	public CaseExpression()
	{
	}

	public CaseExpression(ValueBase condition)
	{
		CaseCondition = condition;
	}

	[Key(1)]
	public ValueBase? CaseCondition { get; init; }

	[Key(2)]
	public List<WhenExpression> WhenExpressions { get; init; } = new();

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		if (CaseCondition != null)
		{
			foreach (var item in CaseCondition.GetInternalQueries())
			{
				yield return item;
			}
		}
		foreach (var exp in WhenExpressions)
		{
			foreach (var item in exp.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		var current = Token.Reserved(this, parent, "case");

		yield return current;
		if (CaseCondition != null) foreach (var item in CaseCondition.GetTokens(current)) yield return item;

		foreach (var item in WhenExpressions)
		{
			foreach (var token in item.GetTokens(current)) yield return token;
		}

		yield return Token.Reserved(this, parent, "end");
	}
}