using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class PartitionClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
	public PartitionClause() : base()
	{
	}

	public PartitionClause(List<ValueBase> collection) : base(collection)
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

		var clause = Token.Reserved(this, parent, "partition by");
		yield return clause;

		foreach (var item in base.GetTokens(clause)) yield return item;
	}
}