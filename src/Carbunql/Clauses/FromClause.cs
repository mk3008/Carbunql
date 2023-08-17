using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

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

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Root.GetParameters());
		if (Relations != null)
		{
			foreach (var item in Relations) prm = prm.Merge(item.GetParameters());
		}
		return prm;
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