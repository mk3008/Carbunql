using Carbunql.Analysis.Parser;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
[Union(0, typeof(LiteralValue))]
[Union(1, typeof(AsArgument))]
[Union(2, typeof(BetweenClause))]
[Union(3, typeof(BracketValue))]
[Union(4, typeof(CaseExpression))]
[Union(5, typeof(CastValue))]
[Union(6, typeof(ColumnValue))]
[Union(7, typeof(FromArgument))]
[Union(8, typeof(FunctionValue))]
[Union(9, typeof(InClause))]
[Union(10, typeof(NegativeValue))]
[Union(11, typeof(ParameterValue))]
[Union(12, typeof(QueryContainer))]
[Union(13, typeof(ValueCollection))]
public abstract class ValueBase : IQueryCommandable
{
	public virtual string GetDefaultName() => string.Empty;

	public OperatableValue? OperatableValue { get; set; }

	public ValueBase AddOperatableValue(string @operator, ValueBase value)
	{
		//if (OperatableValue != null) throw new InvalidOperationException();
		if (OperatableValue != null)
		{
			OperatableValue.Value.AddOperatableValue(@operator, value);
			return value;
		}
		OperatableValue = new OperatableValue(@operator, value);
		return value;
	}

	public string RecommendedName { get; set; } = string.Empty;

	public ValueBase AddOperatableValue(string @operator, string value)
	{
		return AddOperatableValue(@operator, ValueParser.Parse(value));
	}

	public IEnumerable<string> GetOperators()
	{
		if (OperatableValue == null) yield break;
		yield return OperatableValue.Operator;
		foreach (var item in OperatableValue.Value.GetOperators()) yield return item;
	}

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = GetParametersCore();
		prm = prm.Merge(OperatableValue?.GetParameters());
		return prm;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in GetInternalQueriesCore())
		{
			yield return item;
		}
		if (OperatableValue != null)
		{
			foreach (var item in OperatableValue.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var item in GetPhysicalTablesCore())
		{
			yield return item;
		}
		if (OperatableValue != null)
		{
			foreach (var item in OperatableValue.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var item in GetCommonTablesCore())
		{
			yield return item;
		}
		if (OperatableValue != null)
		{
			foreach (var item in OperatableValue.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<ValueBase> GetValues()
	{
		yield return this;

		if (OperatableValue != null)
		{
			foreach (var item in OperatableValue.Value.GetValues())
			{
				yield return item;
			}
		}
	}

	protected abstract IDictionary<string, object?> GetParametersCore();

	protected abstract IEnumerable<SelectQuery> GetInternalQueriesCore();

	protected abstract IEnumerable<PhysicalTable> GetPhysicalTablesCore();

	protected abstract IEnumerable<CommonTable> GetCommonTablesCore();

	public abstract IEnumerable<Token> GetCurrentTokens(Token? parent);

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in GetCurrentTokens(parent)) yield return item;

		if (OperatableValue != null)
		{
			foreach (var item in OperatableValue.GetTokens(parent)) yield return item;
		}
	}

	public BracketValue ToBracket()
	{
		return new BracketValue(this);
	}

	public WhereClause ToWhereClause()
	{
		return new WhereClause(this);
	}

	public string ToText()
	{
		return GetTokens(null).ToText();
	}
}