using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

public class AlterTableQuery : IQueryCommandable, ICommentable, ITable
{
	public AlterTableQuery(ITable t)
	{
		Schema = t.Schema;
		Table = t.Table;
	}

	public AlterTableQuery(ITable t, IAlterCommand command)
	{
		Schema = t.Schema;
		Table = t.Table;
		AlterColumnCommand = command;
	}

	public AlterTableQuery(string schema, string table)
	{
		Schema = schema;
		Table = table;
	}

	public AlterTableQuery(string table)
	{
		Table = table;
	}

	public string? Schema { get; init; } = null;

	public string Table { get; init; }

	public string TableFullName => (string.IsNullOrEmpty(Schema)) ? Table : Schema + "." + Table;

	public IAlterCommand? AlterColumnCommand { get; set; } = null;

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

		yield return Token.Reserved(this, parent, "alter table");
		yield return new Token(this, parent, TableFullName);

		if (AlterColumnCommand != null)
		{
			foreach (var item in AlterColumnCommand.GetTokens(parent))
			{
				yield return item;
			}
			yield break;
		}

		throw new InvalidOperationException();
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		yield break;
	}
}