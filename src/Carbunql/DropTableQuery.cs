using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class DropTableQuery : IQueryCommandable, ICommentable, ITable
{
	public DropTableQuery(ITable t)
	{
		Schema = t.Schema;
		Table = t.Table;
	}

	[IgnoreMember]
	public CommentClause? CommentClause { get; set; }

	public bool HasIfExists { get; set; } = false;

	public string Schema { get; init; }

	public string Table { get; init; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		yield break;
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		yield break;
	}

	public virtual IEnumerable<QueryParameter> GetParameters()
	{
		yield break;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

		yield return Token.Reserved(this, parent, "drop table");
		if (HasIfExists)
		{
			yield return Token.Reserved(this, parent, "if exists");
		}

		yield return new Token(this, parent, this.GetTableFullName());
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}
}