using Carbunql.Analysis;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

public class ValuesQuery : ReadQuery
{
	public ValuesQuery(List<ValueCollection> rows)
	{
		Rows = rows;
	}

	public ValuesQuery(string query)
	{
		var q = ValuesQueryParser.Parse(query);
		Rows = q.Rows;
		OperatableQuery = q.OperatableQuery;
		OrderClause = q.OrderClause;
		LimitClause = q.LimitClause;
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

	public override IDictionary<string, object?> GetInnerParameters()
	{
		var prm = EmptyParameters.Get();
		Rows.ForEach(x => prm = prm.Merge(x.GetParameters()));
		return prm;
	}

	public SelectQuery ToSelectQuery()
	{
		var lst = GetDefaultColumnAliases();
		return ToSelectQuery(lst);
	}

	public SelectQuery ToSelectQuery(IEnumerable<string> columnAlias)
	{
		var sq = new SelectQuery();
		var f = sq.From(ToSelectableTable(columnAlias));

		foreach (var item in columnAlias) sq.Select(f, item);

		sq.OrderClause = OrderClause;
		sq.LimitClause = LimitClause;

		sq.Parameters = Parameters;

		return sq;
	}

	private List<string> GetDefaultColumnAliases()
	{
		if (!Rows.Any() || Rows.First().Count == 0) throw new Exception();
		var cnt = Rows.First().Count;

		var lst = new List<string>();
		cnt.ForEach(x => lst.Add("c" + x));

		return lst;
	}

	public override SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases)
	{
		var vt = new VirtualTable(this);
		if (columnAliases == null)
		{
			var lst = GetDefaultColumnAliases();
			return vt.ToSelectable("v", lst);
		}
		else
		{
			return vt.ToSelectable("v", columnAliases);
		}
	}

	public override IEnumerable<string> GetColumnNames()
	{
		return Enumerable.Empty<string>();
	}
}