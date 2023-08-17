using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql;

public class InsertQuery : IQueryCommandable, IReturning
{
	public InsertClause? InsertClause { get; set; }

	public ReturningClause? ReturningClause { get; set; }

	public IReadQuery? Query { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Query != null)
		{
			foreach (var item in Query.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Query != null)
		{
			foreach (var item in Query.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IDictionary<string, object?>? Parameters { get; set; }

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Parameters);
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (Query == null) yield break;
		if (InsertClause == null) yield break;

		foreach (var item in InsertClause.GetTokens(parent)) yield return item;
		foreach (var item in Query.GetTokens(parent)) yield return item;

		if (ReturningClause == null) yield break;
		foreach (var item in ReturningClause.GetTokens(parent)) yield return item;
	}
}