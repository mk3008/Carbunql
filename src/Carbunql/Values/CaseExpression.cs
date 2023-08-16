﻿using Carbunql.Clauses;

namespace Carbunql.Values;

public class CaseExpression : ValueBase
{
	public CaseExpression()
	{
	}

	public CaseExpression(ValueBase condition)
	{
		CaseCondition = condition;
	}

	public ValueBase? CaseCondition { get; init; }

	public List<WhenExpression> WhenExpressions { get; init; } = new();

	internal override IEnumerable<SelectQuery> GetSelectQueriesCore()
	{
		if (CaseCondition != null)
		{
			foreach (var item in CaseCondition.GetSelectQueries())
			{
				yield return item;
			}
		}
		foreach (var exp in WhenExpressions)
		{
			foreach (var item in exp.GetSelectQueries())
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