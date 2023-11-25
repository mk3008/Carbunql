using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql;

public class CreateTableQuery : IQueryCommandable
{
	public CreateTableQuery(CreateTableClause createTableClause)
	{
		CreateTableClause = createTableClause;
	}

	public CreateTableClause CreateTableClause { get; init; }

	public string TableFullName => CreateTableClause.Table.GetTableFullName();

	public IReadQuery? Query { get; set; }

	public IDictionary<string, object?>? Parameters { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Query == null) yield break;
		foreach (var item in Query.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Query == null) yield break;
		foreach (var item in Query.GetPhysicalTables())
		{
			yield return item;
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
		if (Query == null) yield break;

		foreach (var item in CreateTableClause.GetTokens(parent)) yield return item;
		var t = new Token(this, parent, "as", isReserved: true);
		yield return t;

		foreach (var item in Query.GetTokens()) yield return item;
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (Query == null) yield break;
		foreach (var item in Query.GetCommonTables()) yield return item;
	}

	public SelectQuery ToSelectQuery()
	{
		if (Query == null) throw new NullReferenceException(nameof(Query));

		var sq = new SelectQuery();
		var (_, t) = sq.From(TableFullName).As("t");

		foreach (var item in Query.GetColumnNames())
		{
			sq.Select(t, item);
		}

		return sq;
	}
}