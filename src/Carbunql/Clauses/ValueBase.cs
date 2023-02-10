using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Clauses;

public abstract class ValueBase : IQueryCommand
{
	public virtual string GetDefaultName() => string.Empty;

	public OperatableValue? OperatableValue { get; private set; }

	public ValueBase AddOperatableValue(string @operator, ValueBase value)
	{
		if (OperatableValue != null) throw new InvalidOperationException();
		OperatableValue = new OperatableValue(@operator, value);
		return value;
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
}