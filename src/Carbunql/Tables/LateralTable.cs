using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Tables;

[MessagePackObject(keyAsPropertyName: true)]
public class LateralTable : TableBase
{
	public LateralTable()
	{
		Table = null!;
	}

	public LateralTable(TableBase table)
	{
		Table = table;
	}

	public TableBase Table { get; init; }

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, "lateral");
		foreach (var item in Table.GetTokens(parent)) yield return item;
	}

	public override IEnumerable<QueryParameter> GetParameters()
	{
		return Table.GetParameters();
	}

	public override IList<string> GetColumnNames()
	{
		if (Table is IReadQuery q)
		{
			var s = q.GetOrNewSelectQuery().SelectClause;
			if (s == null) return base.GetColumnNames();
			return s.Select(x => x.Alias).ToList();
		}
		else
		{
			return base.GetColumnNames();
		}
	}

	public override bool IsSelectQuery => Table.IsSelectQuery;

	public override SelectQuery GetSelectQuery()
	{
		return Table.GetSelectQuery();
	}

	public override IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Table.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Table.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public override IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in Table.GetCommonTables())
		{
			yield return item;
		}
	}
}