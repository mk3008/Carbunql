using Carbunql.Clauses;

namespace Carbunql.Tables;

public class VirtualTable : TableBase
{
	public VirtualTable(SelectQuery query)
	{
		isSelectQuery = true;
		Query = query;

	}
	public VirtualTable(IQueryCommandable query)
	{
		Query = query;
	}

	public IQueryCommandable Query { get; init; }

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Query.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);
	}

	public override IDictionary<string, object?> GetParameters()
	{
		return Query.GetParameters();
	}

	public override IList<string> GetColumnNames()
	{
		if (Query is IReadQuery q)
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

	private bool isSelectQuery = false;

	public override bool IsSelectQuery => isSelectQuery;

	public override SelectQuery GetSelectQuery()
	{
		if (isSelectQuery) return (SelectQuery)Query;
		return base.GetSelectQuery();
	}
}