using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class InClause : ValueBase
{
	public InClause()
	{
		Value = null!;
		Argument = null!;
		IsNegative = false;
	}

	public InClause(ValueBase value, ValueBase argument)
	{
		Value = value;
		if (argument is BracketValue || argument is QueryContainer)
		{
			Argument = argument;
		}
		else
		{
			Argument = new BracketValue(argument);
		}
		IsNegative = false;
	}

	public InClause(ValueBase value, ValueBase argument, bool isNegative)
	{
		Value = value;
		if (argument is BracketValue || argument is QueryContainer)
		{
			Argument = argument;
		}
		else
		{
			Argument = new BracketValue(argument);
		}
		IsNegative = isNegative;
	}

	public ValueBase Value { get; init; }

	public ValueBase Argument { get; init; }

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

	public bool IsNegative { get; set; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;

		if (IsNegative) yield return Token.Reserved(this, parent, "not");

		yield return Token.Reserved(this, parent, "in");
		foreach (var item in Argument.GetTokens(parent)) yield return item;
	}

	protected override IDictionary<string, object?> GetParametersCore()
	{
		var prm = Value.GetParameters();
		prm = prm.Merge(Argument.GetParameters());
		return prm;
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