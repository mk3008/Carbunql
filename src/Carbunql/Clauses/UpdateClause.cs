using Carbunql.Tables;

namespace Carbunql.Clauses;

public class UpdateClause : IQueryCommandable
{
	public UpdateClause(SelectableTable table)
	{
		Table = new SelectableTable(table.Table, table.Alias);
	}

	public SelectableTable Table { get; init; }

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var t = Token.Reserved(this, parent, "update");
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

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in Table.GetCommonTables())
		{
			yield return item;
		}
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		throw new NotImplementedException();
	}
}