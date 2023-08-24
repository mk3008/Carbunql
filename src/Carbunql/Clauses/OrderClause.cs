using Carbunql.Tables;

namespace Carbunql.Clauses;

[MessagePack.MessagePackObject]
public class OrderClause : QueryCommandCollection<IQueryCommandable>, IQueryCommandable
{
	public OrderClause() : base()
	{
	}

	public OrderClause(List<IQueryCommandable> collection) : base(collection)
	{
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var value in Items)
		{
			foreach (var item in value.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var value in Items)
		{
			foreach (var item in value.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!Items.Any()) yield break;

		var clause = Token.Reserved(this, parent, "order by");
		yield return clause;

		foreach (var item in base.GetTokens(clause)) yield return item;
	}
}