using Carbunql.Clauses;

namespace Carbunql.Tables;

public class VirtualTable : TableBase
{
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

	public override IList<string> GetValueNames()
	{
		if (Query is IReadQuery q)
		{
			var s = q.GetOrNewSelectQuery().SelectClause;
			if (s == null) return base.GetValueNames();
			return s.Select(x => x.Alias).ToList();
		}
		else
		{
			return base.GetValueNames();
		}
	}
}