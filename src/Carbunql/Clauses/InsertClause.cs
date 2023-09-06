using Carbunql.Tables;

namespace Carbunql.Clauses;

public class InsertClause : IQueryCommandable
{
	public InsertClause(SelectableTable table)
	{
		Table = table;
	}

	public SelectableTable Table { get; init; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in Table.GetCommonTables())
		{
			yield return item;
		}
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Table.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		return Table.GetParameters();
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Table.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var t = Token.Reserved(this, parent, "insert into");
		yield return t;
		foreach (var item in Table.GetTokens(t)) yield return item;
	}
}