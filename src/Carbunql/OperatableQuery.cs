namespace Carbunql;

/// <summary>
/// query with operator
/// </summary>
public class OperatableQuery : IQueryCommandable
{
	public OperatableQuery(string @operator, IReadQuery query)
	{
		Operator = @operator;
		Query = query;
	}

	/// <summary>
	/// "union", "union all", etc.
	/// </summary>
	public string Operator { get; init; }

	public IReadQuery Query { get; init; }

	public IDictionary<string, object?> GetParameters()
	{
		return Query.GetParameters();
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		var current = Token.Reserved(this, parent, Operator);
		yield return current;
		foreach (var item in Query.GetTokens(current)) yield return item;
	}
}