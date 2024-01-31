using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class AtTimeZoneClause : ValueBase
{
	public AtTimeZoneClause()
	{
		Value = null!;
		TimeZone = null!;
	}

	public AtTimeZoneClause(ValueBase value, ValueBase timeZone)
	{
		Value = value;
		TimeZone = timeZone;
	}

	public ValueBase Value { get; init; }

	public ValueBase TimeZone { get; init; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in TimeZone.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		yield return Token.Reserved(this, parent, "at time zone");
		foreach (var item in TimeZone.GetTokens(parent)) yield return item;
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		foreach (var item in Value.GetParameters())
		{
			yield return item;
		}
		foreach (var item in TimeZone.GetParameters())
		{
			yield return item;
		}
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
		foreach (var item in TimeZone.GetPhysicalTables())
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
		foreach (var item in TimeZone.GetCommonTables())
		{
			yield return item;
		}
	}
}