using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class QueryContainer : ValueBase
{
	public QueryContainer()
	{
		Query = null!;
	}

	public QueryContainer(IQueryCommandable query)
	{
		Query = query;
	}

	[Key(1)]
	public IQueryCommandable Query { get; init; }

	internal override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		if (Query is SelectQuery sq)
		{
			foreach (var item in sq.GetInternalQueries())
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