using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class AlterTableQuery : QueryCommandCollection<IAlterCommand>, IQueryCommandable, ICommentable
{
	public AlterTableQuery(AlterTableClause clause)
	{
		AlterTableClause = clause;
	}

	public AlterTableClause AlterTableClause { get; set; }

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

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

		foreach (var item in AlterTableClause.GetTokens(parent))
		{
			yield return item;
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}
}