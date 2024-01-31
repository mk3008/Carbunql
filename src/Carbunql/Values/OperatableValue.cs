using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class OperatableValue : IQueryCommandable
{
	public OperatableValue(string @operator, ValueBase value)
	{
		Operator = @operator;
		Value = value;
	}

	public string Operator { get; init; }

	public ValueBase Value { get; init; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in Value.GetCommonTables())
		{
			yield return item;
		}
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		return Value.GetParameters();
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!string.IsNullOrEmpty(Operator))
		{
			yield return Token.Reserved(this, parent, Operator);
		}
		foreach (var item in Value.GetTokens(parent)) yield return item;
	}
}