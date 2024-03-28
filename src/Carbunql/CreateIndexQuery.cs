using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class CreateIndexQuery : IAlterIndexQuery
{
	public CreateIndexQuery(IndexOnClause clause)
	{
		OnClause = clause;
	}

	public bool IsUnique { get; set; } = false;

	public bool HasIfNotExists { get; set; } = false;

	public string? IndexName { get; init; } = null;

	public IndexOnClause OnClause { get; set; }

	public WhereClause? WhereClause { get; set; } = null;

	[IgnoreMember]
	public CommentClause? CommentClause { get; set; }

	public string Schema => OnClause.Schema;

	public string Table => OnClause.Table;

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

	private Token GetCreateIndexToken(Token? parent)
	{
		if (IsUnique)
		{
			return Token.Reserved(this, parent, "create unique index");
		}
		return Token.Reserved(this, parent, "create index");
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		//if (Query == null) throw new NullReferenceException(nameof(Query));

		if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

		var ct = GetCreateIndexToken(parent);
		yield return ct;

		if (HasIfNotExists)
		{
			yield return Token.Reserved(this, parent, "if not exists");
		}

		if (!string.IsNullOrEmpty(IndexName))
		{
			yield return new Token(this, parent, IndexName);
		}

		if (OnClause != null)
		{
			foreach (var item in OnClause.GetTokens(parent))
			{
				yield return item;
			}
		}

		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetTokens(parent))
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}
}