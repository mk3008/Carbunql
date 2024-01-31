using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class NegativeValue : ValueBase
{
	public NegativeValue()
	{
		Inner = null!;
	}

	public NegativeValue(ValueBase inner)
	{
		Inner = inner;
	}

	public ValueBase Inner { get; init; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Inner.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, "not");
		foreach (var item in Inner.GetTokens(parent)) yield return item;
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		return Inner.GetParameters();
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Inner.GetPhysicalTables())
		{
			yield return item;
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		foreach (var item in Inner.GetCommonTables())
		{
			yield return item;
		}
	}
}