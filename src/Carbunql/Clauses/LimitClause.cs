using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Clauses;

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

	public ValueBase Condition { get; init; }

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