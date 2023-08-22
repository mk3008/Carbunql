using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject]
public class LimitClause : IQueryCommandable
{
	public LimitClause(string text)
	{
		Condition = new LiteralValue(text);
	}

	public LimitClause(ValueBase item)
	{
		Condition = item;
	}

	public LimitClause(List<ValueBase> conditions)
	{
		var lst = new ValueCollection();
		conditions.ForEach(x => lst.Add(x));
		Condition = lst;
	}

	[Key(0)]
	public ValueBase Condition { get; init; }

	[Key(1)]
	public ValueBase? Offset { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Condition.GetInternalQueries())
		{
			yield return item;
		}
		if (Offset != null)
		{
			foreach (var item in Offset.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Condition.GetPhysicalTables())
		{
			yield return item;
		}
		if (Offset != null)
		{
			foreach (var item in Offset.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		var prm = Condition.GetParameters();
		return prm.Merge(Offset?.GetParameters());
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var clause = Token.Reserved(this, parent, "limit");
		yield return clause;

		foreach (var item in Condition.GetTokens(clause)) yield return item;
		if (Offset != null)
		{
			yield return Token.Reserved(this, clause, "offset");
			foreach (var item in Offset.GetTokens(clause)) yield return item;
		}
	}
}