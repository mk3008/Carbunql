using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

public class ValuesQuery : ReadQuery
{
	public ValuesQuery(List<ValueCollection> rows)
	{
		Rows = rows;
	}

	public List<ValueCollection> Rows { get; init; } = new();

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		var clause = Token.Reserved(this, parent, "values");
		yield return clause;

		var isFirst = true;
		foreach (var item in Rows)
		{
			if (isFirst)
			{
				isFirst = false;
			}
			else
			{
				yield return Token.Comma(this, clause);
			}
			var bracket = Token.ReservedBracketStart(this, clause);
			yield return bracket;
			foreach (var token in item.GetTokens(bracket)) yield return token;
			yield return Token.ReservedBracketEnd(this, clause);
		}
	}

	public override WithClause? GetWithClause() => null;

	public override SelectClause? GetSelectClause() => null;

	public override SelectQuery GetOrNewSelectQuery()
	{
		return ToSelectQuery();
	}

	public SelectQuery ToSelectQuery()
	{
		if (!Rows.Any() || Rows.First().Count() == 0) throw new Exception();
		var cnt = Rows.First().Count();

		var columnAlias = new ValueCollection();
		cnt.ForEach(x => columnAlias.Add(new LiteralValue("c" + x)));

		var sq = new SelectQuery();
		sq.SelectClause = new SelectClause();

		var vt = new VirtualTable(this);
		var st = new SelectableTable(vt, "v", columnAlias);
		if (st.ColumnAliases == null) throw new Exception();

		sq.FromClause = new FromClause(st); ;

		foreach (var item in st.ColumnAliases)
		{
			var c = new ColumnValue("v", item.ToText());
			sq.SelectClause.Add(new SelectableItem(c, c.GetDefaultName()));
		}

		return sq;
	}
}