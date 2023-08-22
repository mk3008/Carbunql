using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Clauses;

[MessagePack.MessagePackObject]
public class CommonTable : SelectableTable
{
	public CommonTable(TableBase table, string alias) : base(table, alias)
	{
	}

	public CommonTable(TableBase table, string alias, ValueCollection columnAliases) : base(table, alias, columnAliases)
	{
	}

	[MessagePack.Key(3)]
	public Materialized Materialized { get; set; } = Materialized.Undefined;

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in GetAliasTokens(parent)) yield return item;
		yield return Token.Reserved(this, parent, "as");

		if (Materialized != Materialized.Undefined)
		{
			yield return Token.Reserved(this, parent, Materialized.ToCommandText());
		}

		foreach (var item in Table.GetTokens(parent)) yield return item;
	}

	[MessagePack.Key(4)]
	public bool IsSelectQuery => Table.IsSelectQuery;

	public SelectQuery GetSelectQuery() => Table.GetSelectQuery();
}