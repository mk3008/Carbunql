using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Values;

public class LikeClause : ValueBase
{
	public LikeClause(ValueBase value, ValueBase argument, bool isNegative = false)
	{
		Value = value;
		Argument = argument;
		IsNegative = isNegative;
	}

	public ValueBase Value { get; init; }

	public ValueBase Argument { get; init; }

	public bool IsNegative { get; set; }

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in Argument.GetInternalQueries())
		{
			yield return item;
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		if (IsNegative) yield return Token.Reserved(this, parent, "not");

		yield return Token.Reserved(this, parent, "like");
		foreach (var item in Argument.GetTokens(parent)) yield return item;
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		foreach (var item in Value.GetParameters())
		{
			yield return item;
		}
		foreach (var item in Argument.GetParameters())
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
		foreach (var item in Argument.GetPhysicalTables())
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
		foreach (var item in Argument.GetCommonTables())
		{
			yield return item;
		}
	}
}