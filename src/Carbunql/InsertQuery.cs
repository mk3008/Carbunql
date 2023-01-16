using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class InsertQuery : IQueryCommandable
{
	public InsertClause? InsertClause { get; set; }

	public IReadQuery? Query { get; set; }

	public IDictionary<string, object?>? Parameters { get; set; }

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Parameters);
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (InsertClause == null) throw new NullReferenceException();

		if (Query != null)
		{
			var clause = Query.GetWithClause();
			if (clause != null) foreach (var item in clause.GetTokens(parent)) yield return item;
		}

		foreach (var item in InsertClause.GetTokens(parent)) yield return item;
		if (Query != null)
		{
			var q = Query.GetQuery();
			if (q != null) foreach (var item in q.GetTokens(parent)) yield return item;
		}
	}
}