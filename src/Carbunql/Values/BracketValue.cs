using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class BracketValue : ValueBase
{
	public BracketValue()
	{
		Inner = null!;
	}

	public BracketValue(ValueBase inner)
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

	protected override IDictionary<string, object?> GetParametersCore()
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

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		if (Inner == null) yield break;

		var bracket = Token.ExpressionBracketStart(this, parent);
		yield return bracket;
		foreach (var item in Inner.GetTokens(bracket)) yield return item;
		yield return Token.ExpressionBracketEnd(this, parent);
	}
}