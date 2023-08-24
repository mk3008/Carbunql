﻿using Carbunql.Tables;

namespace Carbunql.Clauses;

[MessagePack.MessagePackObject]
public class WhereClause : IQueryCommandable
{
	public WhereClause(ValueBase condition)
	{
		Condition = condition;
	}

	[MessagePack.Key(0)]
	public ValueBase Condition { get; init; }

	public IDictionary<string, object?> GetParameters()
	{
		return Condition.GetParameters();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var clause = Token.Reserved(this, parent, "where");
		yield return clause;
		foreach (var item in Condition.GetTokens(clause)) yield return item;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Condition.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Condition.GetPhysicalTables())
		{
			yield return item;
		}
	}
}