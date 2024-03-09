using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class DropIndexQuery : IAlterIndexQuery
{
	public DropIndexQuery(string indexName)
	{
		IndexName = indexName;
	}

	public string IndexName { get; init; }

	[IgnoreMember]
	public CommentClause? CommentClause { get; set; }

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
		//if (Query == null) throw new NullReferenceException(nameof(Query));

		if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

		yield return Token.Reserved(this, parent, "drop index");
		yield return new Token(this, parent, IndexName);
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}
}