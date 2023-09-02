using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName:true)]
public class WindowDefinition: IQueryCommand
{
	public WindowDefinition()
	{
	}

	public WindowDefinition(PartitionClause partitionby)
	{
		PartitionBy = partitionby;
	}

	public WindowDefinition(OrderClause orderBy)
	{
		OrderBy = orderBy;
	}

	public WindowDefinition(PartitionClause partitionby, OrderClause orderBy)
	{
		PartitionBy = partitionby;
		OrderBy = orderBy;
	}

	public PartitionClause? PartitionBy { get; set; }

	public OrderClause? OrderBy { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (PartitionBy == null && OrderBy == null) yield break;

		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetTokens(parent)) yield return item;
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetTokens(parent)) yield return item;
		}
	}
}
