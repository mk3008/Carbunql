using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class WithoutTimeZoneClause : ValueBase
{
	public WithoutTimeZoneClause()
	{
		Value = null!;
	}

	public WithoutTimeZoneClause(ValueBase value)
	{
		Value = value;
	}

	public ValueBase Value { get; init; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		yield return Token.Reserved(this, parent, "without time zone");
	}

	protected override IDictionary<string, object?> GetParametersCore()
	{
		var prm = Value.GetParameters();
		return prm;
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		foreach (var item in Value.GetCommonTables())
		{
			yield return item;
		}
	}
}