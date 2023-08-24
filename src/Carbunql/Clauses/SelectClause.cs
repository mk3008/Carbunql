using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject]
public class SelectClause : QueryCommandCollection<SelectableItem>, IQueryCommandable
{
	public SelectClause()
	{
	}

	public SelectClause(List<SelectableItem> collection)
	{
		Items.AddRange(collection);
	}

	[Key(1)]
	public bool HasDistinctKeyword { get; set; } = false;

	[Key(2)]
	public ValueBase? Top { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (Top != null)
		{
			foreach (var item in Top.GetInternalQueries())
			{
				yield return item;
			}
		}

		foreach (var value in Items)
		{
			foreach (var item in value.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (Top != null)
		{
			foreach (var item in Top.GetPhysicalTables())
			{
				yield return item;
			}
		}

		foreach (var value in Items)
		{
			foreach (var item in value.GetPhysicalTables())
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