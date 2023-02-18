using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class OrderClause : QueryCommandCollection<IQueryCommandable>, IQueryCommandable
{
	public OrderClause() : base()
	{
	}

	public OrderClause(List<IQueryCommandable> collection) : base(collection)
	{
	}

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		foreach (var item in this)
		{
			prm = prm.Merge(item.GetParameters());
		}
		return prm;
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Items.Any()) yield break;

		var clause = Token.Reserved(this, parent, "order by");
		yield return clause;

		foreach (var item in base.GetTokens(clause)) yield return item;
	}
}