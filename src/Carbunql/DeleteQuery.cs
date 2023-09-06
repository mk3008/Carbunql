using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql;

public class DeleteQuery : IQueryCommandable, IReturning
{
	public DeleteClause? DeleteClause { get; set; }

	public WithClause? WithClause { get; set; }

	public WhereClause? WhereClause { get; set; }

	public ReturningClause? ReturningClause { get; set; }

	public IDictionary<string, object?>? Parameters { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (DeleteClause != null)
		{
			foreach (var item in DeleteClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (ReturningClause != null)
		{
			foreach (var item in ReturningClause.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (DeleteClause != null)
		{
			foreach (var item in DeleteClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		if (ReturningClause != null)
		{
			foreach (var item in ReturningClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Parameters);
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (DeleteClause == null) throw new NullReferenceException();

		if (WithClause != null) foreach (var item in WithClause.GetTokens(parent)) yield return item;
		foreach (var item in DeleteClause.GetTokens(parent)) yield return item;
		if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;

		if (ReturningClause == null) yield break;
		foreach (var item in ReturningClause.GetTokens(parent)) yield return item;
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (DeleteClause != null)
		{
			foreach (var item in DeleteClause.GetCommonTables())
			{
				yield return item;
			}
		}
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetCommonTables())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetCommonTables())
			{
				yield return item;
			}
		}
		if (ReturningClause != null)
		{
			foreach (var item in ReturningClause.GetCommonTables())
			{
				yield return item;
			}
		}
	}
}