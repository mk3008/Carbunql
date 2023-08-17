using Carbunql.Tables;

namespace Carbunql.Clauses;

public class ReturningClause : IQueryCommand
{
	public ReturningClause(ValueBase value)
	{
		Value = value;
	}

	public ValueBase Value { get; init; }

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var t = Token.Reserved(this, parent, "returning");
		yield return t;
		foreach (var item in Value.GetTokens(t)) yield return item;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
	}
}