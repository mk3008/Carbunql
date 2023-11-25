using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

public class CreateTableClause : IQueryCommandable
{
	public CreateTableClause()
	{
	}

	public CreateTableClause(TableBase table)
	{
		Table = table;
	}

	public bool IsTemporary { get; set; } = true;

	public TableBase Table { get; set; } = null!;

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (Table != null)
		{
			foreach (var item in Table.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Table != null)
		{
			foreach (var item in Table.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Table != null)
		{
			foreach (var item in Table.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Table?.GetParameters());
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (Table == null) throw new Exception();

		Token clause = GetClauseToken(parent);
		yield return clause;

		foreach (var item in Table.GetTokens(clause)) yield return item;
	}

	private Token GetClauseToken(Token? parent)
	{
		if (IsTemporary)
		{
			return Token.Reserved(this, parent, "create temporary table");
		}
		return Token.Reserved(this, parent, "create table");
	}
}