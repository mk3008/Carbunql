﻿using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class WhenExpression : IQueryCommandable
{
	public WhenExpression(ValueBase condition, ValueBase value)
	{
		Condition = condition;
		Value = value;
	}

	public WhenExpression(ValueBase value)
	{
		Value = value;
	}

	public ValueBase? Condition { get; init; }

	public ValueBase Value { get; private set; }

	public void SetValue(ValueBase value)
	{
		Value = value;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetInternalQueries())
			{
				yield return item;
			}
		}
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (Condition != null)
		{
			yield return Token.Reserved(this, parent, "when");
			foreach (var item in Condition.GetTokens(parent)) yield return item;
			yield return Token.Reserved(this, parent, "then");
			foreach (var item in Value.GetTokens(parent)) yield return item;
		}
		else
		{
			yield return Token.Reserved(this, parent, "else");
			foreach (var item in Value.GetTokens(parent)) yield return item;
		}
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetParameters())
			{
				yield return item;
			}
		}
		foreach (var item in Value.GetParameters())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetPhysicalTables())
			{
				yield return item;
			}
		}
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (Condition != null)
		{
			foreach (var item in Condition.GetCommonTables())
			{
				yield return item;
			}
		}
		foreach (var item in Value.GetCommonTables())
		{
			yield return item;
		}
	}
}