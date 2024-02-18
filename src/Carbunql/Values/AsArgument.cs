using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class AsArgument : ValueBase
{
	public AsArgument()
	{
		Value = null!;
		Type = null!;
	}

	public AsArgument(ValueBase value, ValueBase type)
	{
		Value = value;
		Type = type;
	}

	public ValueBase Value { get; init; }

	public ValueBase Type { get; init; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Type.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "as");
		foreach (var item in Type.GetTokens(parent)) yield return item;
	}

	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		foreach (var item in Value.GetPhysicalTables())
		{
			yield return item;
		}
		foreach (var item in Type.GetPhysicalTables())
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
		foreach (var item in Type.GetCommonTables())
		{
			yield return item;
		}
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		foreach (var item in Value.GetParameters())
		{
			yield return item;
		}
		foreach (var item in Type.GetParameters())
		{
			yield return item;
		}
	}
}