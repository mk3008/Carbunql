using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class UsingClause : IQueryCommandable
{
	public UsingClause(SelectableTable root, ValueBase condition, IEnumerable<string> keys)
	{
		Root = root;
		Condition = condition;
		Keys = keys;
	}

	public SelectableTable Root { get; init; }

	public ValueBase Condition { get; init; }

	public IEnumerable<string> Keys { get; init; }

	public IEnumerable<SelectQuery> GetSelectQueries()
	{
		foreach (var item in Root.GetSelectQueries())
		{
			yield return item;
		}
		foreach (var item in Condition.GetSelectQueries())
		{
			yield return item;
		}
	}

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(Root.GetParameters());
		if (Condition != null) prm = prm.Merge(Condition.GetParameters());
		return prm;
	}

	private IEnumerable<Token> GetOnTokens(Token? parent)
	{
		yield return Token.Reserved(this, parent, "on");
		foreach (var token in Condition.GetTokens(parent)) yield return token;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		//using
		var t = Token.Reserved(this, parent, "using");
		yield return t;
		foreach (var token in Root.GetTokens(t)) yield return token;

		//on
		foreach (var token in GetOnTokens(t)) yield return token;
	}
}