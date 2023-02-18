using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class FromClause : IQueryCommandable
{
	public FromClause(SelectableTable root)
	{
		Root = root;
	}

	public SelectableTable Root { get; init; }

	public List<Relation>? Relations { get; set; }

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