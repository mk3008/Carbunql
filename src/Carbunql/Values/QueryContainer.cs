using Carbunql.Clauses;

namespace Carbunql.Values;

public class QueryContainer : ValueBase
{
	public QueryContainer(IQueryCommandable query)
	{
		Query = query;
	}

	public IQueryCommandable Query { get; init; }

	internal override IEnumerable<SelectQuery> GetSelectQueriesCore()
	{
		if (Query is SelectQuery sq)
		{
			foreach (var item in sq.GetSelectQueries())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Query.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);
	}
}