using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class WindowDefinition : IQueryCommandable
{
	public WindowDefinition()
	{
	}

	public WindowDefinition(string name)
	{
		Name = name;
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

	public string Name { get; set; } = string.Empty;

	public PartitionClause? PartitionBy { get; set; }

	public OrderClause? OrderBy { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (!string.IsNullOrEmpty(Name)) yield break;

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

	public IEnumerable<QueryParameter> GetParameters()
	{
		if (!string.IsNullOrEmpty(Name)) yield break;
		if (PartitionBy == null && OrderBy == null) yield break;

		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetParameters())
			{
				yield return item;
			}
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetParameters())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (!string.IsNullOrEmpty(Name)) yield break;
		if (PartitionBy == null && OrderBy == null) yield break;

		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (!string.IsNullOrEmpty(Name)) yield break;
		if (PartitionBy == null && OrderBy == null) yield break;

		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetCommonTables())
			{
				yield return item;
			}
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!string.IsNullOrEmpty(Name))
		{
			yield return new Token(this, parent, Name);
			yield break;
		}

		if (PartitionBy == null && OrderBy == null) yield break;

		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;

		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetTokens(bracket))
			{
				yield return item;
			}
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetTokens(bracket))
			{
				yield return item;
			}
		}

		yield return Token.ReservedBracketEnd(this, parent);
	}
}
