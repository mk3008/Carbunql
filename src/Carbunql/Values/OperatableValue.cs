using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject]
public class OperatableValue : IQueryCommand
{
	public OperatableValue(string @operator, ValueBase value)
	{
		Operator = @operator;
		Value = value;
	}

	[Key(0)]
	public string Operator { get; init; }

	[Key(1)]
	public ValueBase Value { get; init; }

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

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (!string.IsNullOrEmpty(Operator))
		{
			yield return Token.Reserved(this, parent, Operator);
		}
		foreach (var item in Value.GetTokens(parent)) yield return item;
	}
}