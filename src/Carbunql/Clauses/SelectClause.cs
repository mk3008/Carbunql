using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class SelectClause : QueryCommandCollection<SelectableItem>, IQueryCommandable
{
	public SelectClause()
	{
	}

	public SelectClause(List<SelectableItem> collection)
	{
		Items.AddRange(collection);
	}

	public bool HasDistinctKeyword { get; set; } = false;

	public ValueBase? Top { get; set; }

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		foreach (var item in Items)
		{
			prm = prm.Merge(item.GetParameters());
		}
		return prm;
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		Token clause = GetClauseToken(parent);
		yield return clause;

		foreach (var item in base.GetTokens(clause)) yield return item;
	}

	private Token GetClauseToken(Token? parent)
	{
		if (HasDistinctKeyword && Top != null)
		{
			return Token.Reserved(this, parent, "select distinct top " + Top.GetTokens(parent).ToText());
		}
		else if (HasDistinctKeyword)
		{
			return Token.Reserved(this, parent, "select distinct");
		}
		else if (Top != null)
		{
			return Token.Reserved(this, parent, "select top " + Top.GetTokens(parent).ToText());
		}
		return Token.Reserved(this, parent, "select");
	}
}