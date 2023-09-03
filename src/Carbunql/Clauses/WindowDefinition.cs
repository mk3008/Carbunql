using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class WindowDefinition : IQueryCommandable
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

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		if (PartitionBy == null && OrderBy == null) return prm;

		if (PartitionBy != null)
		{
			prm = prm.Merge(PartitionBy.GetParameters());
		}
		if (OrderBy != null)
		{
			prm = prm.Merge(OrderBy.GetParameters());
		}
		return prm;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (PartitionBy == null && OrderBy == null) yield break;

		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetPhysicalTables()) yield return item;
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetPhysicalTables()) yield return item;
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
