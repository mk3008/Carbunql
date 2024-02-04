using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class TopClause : IQueryCommandable
{
	public TopClause(ValueBase value)
	{
		Value = value;
	}

	public ValueBase Value { get; init; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		return Value.GetCommonTables();
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		return Value.GetInternalQueries();
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		return Value.GetParameters();
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		return Value.GetPhysicalTables();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, "top");

		foreach (var item in Value.GetTokens(parent))
		{
			yield return item;
		}
	}
}
