using Carbunql.Analysis;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;

namespace Carbunql;

public class SelectQuery : ReadQuery, IQueryCommandable
{
	public SelectQuery() { }

	public SelectQuery(string query)
	{
		var q = SelectQueryParser.Parse(query);
		WithClause = q.WithClause;
		SelectClause = q.SelectClause;
		FromClause = q.FromClause;
		WhereClause = q.WhereClause;
		GroupClause = q.GroupClause;
		HavingClause = q.HavingClause;
		OperatableQuery = q.OperatableQuery;
		OrderClause = q.OrderClause;
		LimitClause = q.LimitClause;
	}

	public WithClause? WithClause { get; set; } = new();

	public SelectClause? SelectClause { get; set; }

	public FromClause? FromClause { get; set; }

	public WhereClause? WhereClause { get; set; }

	public GroupClause? GroupClause { get; set; }

	public HavingClause? HavingClause { get; set; }

	public override IEnumerable<Token> GetCurrentTokens(Token? parent)
	{
		if (SelectClause == null) yield break;

		if (parent == null && WithClause != null) foreach (var item in WithClause.GetTokens()) yield return item;
		foreach (var item in SelectClause.GetTokens(parent)) yield return item;
		if (FromClause != null) foreach (var item in FromClause.GetTokens(parent)) yield return item;
		if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;
		if (GroupClause != null) foreach (var item in GroupClause.GetTokens(parent)) yield return item;
		if (HavingClause != null) foreach (var item in HavingClause.GetTokens(parent)) yield return item;
	}

	public override WithClause? GetWithClause() => WithClause;

	public override SelectClause? GetSelectClause() => SelectClause;

	public override SelectQuery GetOrNewSelectQuery() => this;

	public override IDictionary<string, object?> GetInnerParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(FromClause?.GetParameters());
		prm = prm.Merge(WhereClause?.GetParameters());
		prm = prm.Merge(GroupClause?.GetParameters());
		prm = prm.Merge(HavingClause?.GetParameters());
		return prm;
	}

	public override SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases)
	{
		var vt = new VirtualTable(this);
		if (columnAliases != null)
		{
			return new SelectableTable(vt, "q", columnAliases.ToValueCollection());
		}
		return new SelectableTable(vt, "q");
	}

	public override IEnumerable<string> GetColumnNames()
	{
		if (SelectClause == null) return Enumerable.Empty<string>();
		return SelectClause.Select(x => x.Alias);
	}

	public override IEnumerable<SelectableTable> GetSelectableTables(bool cascade = false)
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetSelectableTables(cascade))
			{
				yield return item;
			}
		}
		if (FromClause != null)
		{
			foreach (var item in FromClause.GetSelectableTables(cascade))
			{
				yield return item;
			}
		}
		if (OperatableQuery != null)
		{
			foreach (var item in OperatableQuery.GetSelectableTables(cascade))
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<string> GetPhysicalTables()
	{
		var commontables = ((WithClause != null) ? WithClause.Select(x => x.Alias) : Enumerable.Empty<string>()).ToList();
		var tables = GetSelectableTables(true).Select(x => x.Table.GetTableFullName()).Distinct().Where(x => !string.IsNullOrEmpty(x));
		return tables.Where(x => !commontables.Contains(x));
	}

	public override IEnumerable<SelectQuery> GetSelectQueries()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetSelectQueries())
			{
				yield return item;
			}
		}

		yield return this;

		if (SelectClause != null)
		{
			foreach (var item in SelectClause.GetSelectQueries())
			{
				yield return item;
			}
		}
		if (FromClause != null)
		{
			foreach (var item in FromClause.GetSelectQueries())
			{
				yield return item;
			}
		}
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetSelectQueries())
			{
				yield return item;
			}
		}
		if (GroupClause != null)
		{
			foreach (var item in GroupClause.GetSelectQueries())
			{
				yield return item;
			}
		}
		if (HavingClause != null)
		{
			foreach (var item in HavingClause.GetSelectQueries())
			{
				yield return item;
			}
		}
		if (OperatableQuery != null)
		{
			foreach (var item in OperatableQuery.GetSelectQueries())
			{
				yield return item;
			}
		}
		if (OrderClause != null)
		{
			foreach (var item in OrderClause.GetSelectQueries())
			{
				yield return item;
			}
		}
		if (LimitClause != null)
		{
			foreach (var item in LimitClause.GetSelectQueries())
			{
				yield return item;
			}
		}
	}
}
