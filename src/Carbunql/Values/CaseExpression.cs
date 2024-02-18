using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class CaseExpression : ValueBase
{
	public CaseExpression()
	{
	}

	public CaseExpression(ValueBase condition)
	{
		CaseCondition = condition;
	}

	public ValueBase? CaseCondition { get; init; }

	public List<WhenExpression> WhenExpressions { get; init; } = new();

	protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		if (CaseCondition != null)
		{
			foreach (var item in CaseCondition.GetInternalQueries())
			{
				yield return item;
			}
		}
		foreach (var exp in WhenExpressions)
		{
			foreach (var item in exp.GetInternalQueries())
			{
				yield return item;
			}
		}
	}
	protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		if (CaseCondition != null)
		{
			foreach (var item in CaseCondition.GetPhysicalTables())
			{
				yield return item;
			}
		}
		foreach (var exp in WhenExpressions)
		{
			foreach (var item in exp.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	protected override IEnumerable<CommonTable> GetCommonTablesCore()
	{
		if (CaseCondition != null)
		{
			foreach (var item in CaseCondition.GetCommonTables())
			{
				yield return item;
			}
		}
		foreach (var exp in WhenExpressions)
		{
			foreach (var item in exp.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		var current = Token.Reserved(this, parent, "case");

		yield return current;
		if (CaseCondition != null) foreach (var item in CaseCondition.GetTokens(current)) yield return item;

		foreach (var item in WhenExpressions)
		{
			foreach (var token in item.GetTokens(current)) yield return token;
		}

		yield return Token.Reserved(this, parent, "end");
	}

	protected override IEnumerable<QueryParameter> GetParametersCore()
	{
		if (CaseCondition != null)
		{
			foreach (var item in CaseCondition.GetParameters())
			{
				yield return item;
			}
		}
		foreach (var item in WhenExpressions)
		{
			foreach (var p in item.GetParameters())
			{
				yield return p;
			}
		}
	}
}