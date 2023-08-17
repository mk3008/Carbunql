namespace Carbunql.Clauses;

public class SelectableItem : IQueryCommandable, ISelectable
{
	public SelectableItem(ValueBase value, string alias)
	{
		Value = value;
		Alias = alias;
	}

	public ValueBase Value { get; init; }

	public string Alias { get; private set; }

	public void SetAlias(string alias)
	{
		Alias = alias;
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var item in Value.GetInternalQueries())
		{
			yield return item;
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in Value.GetTokens(parent)) yield return item;
		if (!string.IsNullOrEmpty(Alias) && Alias != Value.GetDefaultName())
		{
			yield return Token.Reserved(this, parent, "as");
			yield return new Token(this, parent, Alias);
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		return Value.GetParameters();
	}
}