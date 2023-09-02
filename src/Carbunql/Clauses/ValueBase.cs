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
		if (OperatableValue != null) throw new InvalidOperationException();
		OperatableValue = new OperatableValue(@operator, value);
		return value;
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

	internal virtual IEnumerable<SelectQuery> GetInternalQueriesCore()
	{
		yield break;
	}

	internal virtual IEnumerable<PhysicalTable> GetPhysicalTablesCore()
	{
		yield break;
	}

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

	public virtual IDictionary<string, object?> GetParameters()
	{
		return EmptyParameters.Get();
	}
}