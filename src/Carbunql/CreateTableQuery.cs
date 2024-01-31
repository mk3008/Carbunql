using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class CreateTableQuery : IQueryCommandable, ICommentable
{
	public CreateTableQuery(CreateTableClause createTableClause)
	{
		CreateTableClause = createTableClause;
	}

	public CreateTableClause CreateTableClause { get; init; }

	[IgnoreMember]
	public CommentClause? CommentClause { get; set; }

	public string TableFullName => CreateTableClause.Table.GetTableFullName();

	public IReadQuery? Query { get; set; }

	public IEnumerable<QueryParameter>? Parameters { get; set; }

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

	public virtual IEnumerable<QueryParameter> GetParameters()
	{
		if (Parameters != null)
		{
			foreach (var item in Parameters)
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (Query == null) throw new NullReferenceException(nameof(Query));

		if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

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
		if (string.IsNullOrEmpty(TableFullName)) throw new NullReferenceException(nameof(TableFullName));

		var sq = new SelectQuery();
		var (_, t) = sq.From(TableFullName).As("t");

		foreach (var item in Query.GetColumnNames())
		{
			sq.Select(t, item);
		}

		return sq;
	}

	public SelectQuery ToCountQuery(string alias = "row_count")
	{
		if (string.IsNullOrEmpty(TableFullName)) throw new NullReferenceException(nameof(TableFullName));

		var sq = new SelectQuery();
		sq.From(TableFullName).As("q");
		sq.Select("count(*)").As(alias);
		return sq;
	}
}