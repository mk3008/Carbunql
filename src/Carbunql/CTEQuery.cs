using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class CTEQuery : IReadQuery
{
	public CTEQuery()
	{
	}

	public CTEQuery(WithClause with)
	{
		WithClause = with;
	}

	public WithClause WithClause { get; } = new();

	public SelectQuery? Query { get; set; }

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (Query == null) throw new NullReferenceException(nameof(Query));

		foreach (var item in WithClause.GetTokens(parent)) yield return item;
		foreach (var item in Query.GetTokens(parent)) yield return item;
	}

	public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();

	public virtual IDictionary<string, object?> GetParameters()
	{
		if (Query == null) throw new NullReferenceException(nameof(Query));

		var prm = EmptyParameters.Get();
		prm = prm.Merge(Parameters);
		prm = prm.Merge(Query.GetParameters());
		return prm;
	}

	public SelectClause? GetSelectClause() => GetQuery().GetSelectClause();

	public ReadQuery GetQuery()
	{
		if (Query == null) throw new NullReferenceException(nameof(Query));
		return Query;
	}

	public WithClause? GetWithClause()
	{
		return WithClause;
	}
}