using Carbunql.Tables;

namespace Carbunql.Clauses;

[MessagePack.MessagePackObject]
public class HavingClause : IQueryCommandable
{
	public HavingClause(ValueBase condition)
	{
		Condition = condition;
	}

	[MessagePack.Key(0)]
	public ValueBase Condition { get; init; }

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

	public IDictionary<string, object?> GetParameters()
	{
		return Condition.GetParameters();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var clause = Token.Reserved(this, parent, "having");
		yield return clause;

		foreach (var item in Condition.GetTokens(clause)) yield return item;
	}
}