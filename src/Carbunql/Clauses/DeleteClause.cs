using Carbunql.Tables;

namespace Carbunql.Clauses;

public class DeleteClause : IQueryCommandable
{
	public DeleteClause(SelectableTable table)
	{
		Table = new SelectableTable(table.Table, table.Alias);
	}

	public SelectableTable Table { get; init; }

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var t = Token.Reserved(this, parent, "delete from");
		yield return t;
		foreach (var item in Table.GetTokens(t)) yield return item;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Table.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Table.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		return Table.GetParameters();
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in Table.GetCommonTables())
		{
			yield return item;
		}
	}
}