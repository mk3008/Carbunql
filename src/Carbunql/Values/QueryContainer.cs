using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
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

	public IQueryCommandable Query { get; init; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Query.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		var bracket = Token.ReservedBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Query.GetTokens(bracket)) yield return item;
		yield return Token.ReservedBracketEnd(this, parent);
	}

	protected override IDictionary<string, object?> GetParametersCore()
	{
		return Query.GetParameters();
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Query.GetPhysicalTables())
		{
			yield return item;
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		foreach (var item in Query.GetCommonTables())
		{
			yield return item;
		}
	}
}