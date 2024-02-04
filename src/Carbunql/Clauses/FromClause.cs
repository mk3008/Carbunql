﻿using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class FromClause : IQueryCommandable
{
	public FromClause(SelectableTable root)
	{
		Root = root;
	}

	public SelectableTable Root { get; init; }

	public List<Relation>? Relations { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Root.GetInternalQueries())
		{
			yield return item;
		}

		if (Relations != null)
		{
			foreach (var relation in Relations)
			{
				foreach (var item in relation.GetInternalQueries())
				{
					yield return item;
				}
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Root.GetPhysicalTables())
		{
			yield return item;
		}

		if (Relations != null)
		{
			foreach (var relation in Relations)
			{
				foreach (var item in relation.GetPhysicalTables())
				{
					yield return item;
				}
			}
		}
	}

	public IEnumerable<SelectableTable> GetSelectableTables()
	{
		yield return Root;

		if (Relations != null)
		{
			foreach (var item in Relations)
			{
				yield return item.Table;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in Root.GetCommonTables())
		{
			yield return item;
		}

		if (Relations != null)
		{
			foreach (var relation in Relations)
			{
				foreach (var item in relation.GetCommonTables())
				{
					yield return item;
				}
			}
		}
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		foreach (var item in Root.GetParameters())
		{
			yield return item;
		}
		if (Relations != null)
		{
			foreach (var relation in Relations)
			{
				foreach (var item in relation.GetParameters())
				{
					yield return item;
				}
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var clause = Token.Reserved(this, parent, "from");

		yield return clause;
		foreach (var item in Root.GetTokens(clause)) yield return item;

		if (Relations == null) yield break;

		foreach (var item in Relations)
		{
			foreach (var token in item.GetTokens(clause)) yield return token;
		}
	}
}