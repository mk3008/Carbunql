using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Clauses;

public class SelectableTable : IQueryCommandable, ISelectable
{
	public SelectableTable(TableBase table, string alias)
	{
		Table = table;
		Alias = alias;
	}

	public SelectableTable(TableBase table, string alias, ValueCollection columnAliases)
	{
		Table = table;
		Alias = alias;
		ColumnAliases = columnAliases;
	}

	public TableBase Table { get; init; }

	public string Alias { get; private set; }

	public void SetAlias(string alias)
	{
		this.Alias = alias;
	}

	public ValueCollection? ColumnAliases { get; init; }

	public IEnumerable<Token> GetAliasTokens(Token? parent)
	{
		if (!string.IsNullOrEmpty(Alias) && Alias != Table.GetDefaultName())
		{
			yield return new Token(this, parent, Alias);
		}

		if (ColumnAliases != null)
		{
			var bracket = Token.ReservedBracketStart(this, parent);
			yield return bracket;
			foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
			yield return Token.ReservedBracketEnd(this, parent);
		}
	}

	public virtual IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in Table.GetTokens(parent)) yield return item;

		if (!string.IsNullOrEmpty(Alias) && Alias != Table.GetDefaultName())
		{
			yield return Token.Reserved(this, parent, "as");
			yield return new Token(this, parent, Alias);
		}

		if (ColumnAliases != null)
		{
			var bracket = Token.ReservedBracketStart(this, parent);
			yield return bracket;
			foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
			yield return Token.ReservedBracketEnd(this, parent);
		}
	}

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = Table.GetParameters();
		prm = prm.Merge(ColumnAliases?.GetParameters());
		return prm;
	}
}