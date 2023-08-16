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

	public IEnumerable<SelectQuery> GetSelectQueries()
	{
		if (Top != null)
		{
			foreach (var item in Top.GetSelectQueries())
			{
				yield return item;
			}
		}

		foreach (var value in Items)
		{
			foreach (var item in value.GetSelectQueries())
			{
				yield return item;
			}
		}
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

	public void FilterInColumns(IEnumerable<string> columns)
	{
		var lst = this.Where(x => !columns.Contains(x.Alias)).ToList();
		foreach (var item in lst) Remove(item);
	}
}