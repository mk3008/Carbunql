using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class FromArgument : ValueBase
{
	public FromArgument()
	{
		Unit = null!;
		Value = null!;
	}

	public FromArgument(ValueBase unit, ValueBase value)
	{
		Unit = unit;
		Value = value;
	}

	public ValueBase Unit { get; init; }

	public ValueBase Value { get; init; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Unit.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Unit.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "from");
		foreach (var item in Value.GetTokens(parent)) yield return item;
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		foreach (var item in Unit.GetParameters())
		{
			yield return item;
		}
		foreach (var item in Value.GetParameters())
		{
			yield return item;
		}
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Unit.GetPhysicalTables())
		{
			yield return item;
		}
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		foreach (var item in Unit.GetCommonTables())
		{
			yield return item;
		}
		foreach (var item in Value.GetCommonTables())
		{
			yield return item;
		}
	}
}