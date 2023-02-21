using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Building;

public static class ReadQueryExtension
{
	public static SelectQuery GetLast(this SelectQuery source)
	{
		if (source.OperatableQuery == null) return source;
		if (source.OperatableQuery.Query is SelectQuery sq) return sq.GetLast();
		return source;
	}

	public static (SelectQuery, CommonTable) ToCTE(this IReadQuery source, string alias)
	{
		var sq = new SelectQuery();
		sq.ImportCommonTable(source);

		sq.WithClause ??= new WithClause();
		var ct = source.ToCommonTable(alias);
		sq.WithClause.Add(ct);

		return (sq, ct);
	}

	public static CommonTable ToCommonTable(this IReadQuery source, string alias)
	{
		return new CommonTable(new VirtualTable(source), alias);
	}

	public static CommonTable ToCommonTable(this IReadQuery source, string alias, IList<string> columnAliases)
	{
		return new CommonTable(new VirtualTable(source), alias, columnAliases.ToValueCollection());
	}

	public static SelectableTable ToSelectableTable(this IReadQuery source)
	{
		return source.ToSelectableTable("t");
	}

	public static SelectableTable ToSelectableTable(this IReadQuery source, string alias)
	{
		var t = new VirtualTable(source);
		return new SelectableTable(t, alias);
	}

	public static FromClause ToFromClause(this IReadQuery source, string alias)
	{
		var t = source.ToSelectableTable(alias);
		return new FromClause(t);
	}

	public static ExistsExpression ToExists(this IReadQuery source)
	{
		return new ExistsExpression(source);
	}

	public static NegativeValue ToNotExists(this IReadQuery source)
	{
		return new NegativeValue(source.ToExists());
	}

	public static SelectQuery ToSubQuery(this IReadQuery source)
	{
		return source.ToSubQuery("q");
	}

	public static SelectQuery ToSubQuery(this IReadQuery source, string alias)
	{
		var sq = new SelectQuery();
		sq.From(source).As(alias);
		return sq;
	}

	public static CreateTableQuery ToCreateTableQuery(this IReadQuery source, string table, bool isTemporary = true)
	{
		var t = table.ToPhysicalTable();
		return source.ToCreateTableQuery(t, isTemporary);
	}

	public static CreateTableQuery ToCreateTableQuery(this IReadQuery source, TableBase table, bool isTemporary)
	{
		return new CreateTableQuery(new CreateTableClause(table) { IsTemporary = isTemporary })
		{
			Parameters = source.GetParameters(),
			Query = source,
		};
	}

	public static SelectableTable ToInsertTable(this IReadQuery source, string table)
	{
		var s = source.GetSelectClause();
		if (s == null)
		{
			return new SelectableTable(new PhysicalTable(table), table);
		}

		var vals = new ValueCollection();
		foreach (var item in s.Items)
		{
			vals.Add(new ColumnValue(item.Alias));
		}

		return new SelectableTable(new PhysicalTable(table), table, vals);
	}

	public static InsertQuery ToInsertQuery(this IReadQuery source, string table)
	{
		var t = source.ToInsertTable(table);
		return source.ToInsertQuery(t);
	}

	public static InsertQuery ToInsertQuery(this IReadQuery source, SelectableTable table)
	{
		return new InsertQuery()
		{
			InsertClause = new InsertClause(table),
			Parameters = source.GetParameters(),
			Query = source,
		};
	}

    /// <summary>
    /// set val = queryAlias.val
    /// </summary>
    /// <param name="source"></param>
    /// <param name="keys"></param>
    /// <param name="alias"></param>
    /// <param name="queryAlias"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private static SetClause ToSetClause(this IReadQuery source, IEnumerable<string> keys, string alias, string queryAlias)
	{
		var s = source.GetSelectClause();
		if (s == null) throw new NotSupportedException("select clause is not found.");
		var cols = s.Items.Where(x => !keys.Contains(x.Alias)).Select(x => x.Alias).ToList();

		var clause = new SetClause();
		foreach (var item in cols)
		{
			var c = new ColumnValue(alias, item);
			c.Equal(new ColumnValue(queryAlias, item));
			clause.Add(c);
		};
		return clause;
	}

	/// <summary>
	/// where alias.key = queryAlias.key
	/// </summary>
	/// <param name="keys"></param>
	/// <param name="alias"></param>
	/// <param name="queryAlias"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private static WhereClause ToWhereClauseAsUpdate(IEnumerable<string> keys, string alias, string queryAlias)
	{
		ValueBase? cnd = null;
		foreach (var item in keys)
		{
			if (cnd == null)
			{
				cnd = new ColumnValue(alias, item);
			}
			else
			{
				cnd.And(new ColumnValue(alias, item));
			}
			cnd.Equal(new ColumnValue(queryAlias, item));
		}
		if (cnd == null) throw new Exception();
		return cnd.ToWhereClause();
	}

	public static UpdateQuery ToUpdateQuery(this IReadQuery source, string table, IEnumerable<string> keys)
	{
		var t = table.ToPhysicalTable().ToSelectable("_t");
		return source.ToUpdateQuery(t, keys);
	}

	public static UpdateQuery ToUpdateQuery(this IReadQuery source, string table, string alias, IEnumerable<string> keys)
	{
		var t = table.ToPhysicalTable().ToSelectable(alias);
		return source.ToUpdateQuery(t, keys);
	}

	public static UpdateQuery ToUpdateQuery(this IReadQuery source, SelectableTable table, IEnumerable<string> keys)
	{
		var queryAlias = "q";

		return new UpdateQuery()
		{
			UpdateClause = new UpdateClause(table),
			SetClause = source.ToSetClause(keys, table.Alias, queryAlias),
			Parameters = source.GetParameters(),
			FromClause = source.ToFromClause(queryAlias),
			WhereClause = ToWhereClauseAsUpdate(keys, table.Alias, queryAlias),
		};
	}

	public static DeleteQuery ToDeleteQuery(this IReadQuery source, string table, IEnumerable<string> keys)
	{
		var t = table.ToPhysicalTable().ToSelectable();
		return source.ToDeleteQuery(t, keys, source.GetWithClause());
	}

	private static WhereClause ToWhereClauseAsDelete(this IReadQuery source, IEnumerable<string> keys, string alias, string queryAlias)
	{
		var ks = keys.ToList();

		var cnd = new ValueCollection();
		ks.ForEach(x => cnd.Add(new ColumnValue(alias, x)));
		if (cnd == null) throw new Exception();

		var sq = new SelectQuery();
		var (f, a)= sq.From(source).As(queryAlias);
		foreach (var item in a.Table.GetValueNames())
		{
			if (!ks.Where(k => k.IsEqualNoCase(item)).Any()) continue;
			sq.Select(a, item);
		}

		var exp = new InExpression(cnd.ToBracket(), sq.ToValue());
		return exp.ToWhereClause();
	}

	public static DeleteQuery ToDeleteQuery(this IReadQuery source, SelectableTable table, IEnumerable<string> keys, WithClause? wclause = null)
	{
		var queryAlias = "q";

		return new DeleteQuery()
		{
			DeleteClause = new DeleteClause(table),
			Parameters = source.GetParameters(),
			WithClause = wclause,
			WhereClause = source.ToWhereClauseAsDelete(keys, table.Alias, queryAlias),
		};
	}

	public static void UnionAll(this IReadQuery source, IReadQuery query)
	{
		var sq = source.GetOrNewSelectQuery();
		sq.GetLast().AddOperatableValue("union all", query);
	}

	public static void UnionAll(this IReadQuery source, Func<IReadQuery> builder)
	{
		source.UnionAll(builder());
	}

	public static void Union(this IReadQuery source, IReadQuery query)
	{
		var sq = source.GetOrNewSelectQuery();
		sq.GetLast().AddOperatableValue("union", query);
	}

	public static void Union(this IReadQuery source, Func<IReadQuery> builder)
	{
		source.Union(builder());
	}

    public static SelectQuery ToCountQuery(this IReadQuery source, string alias = "row_count")
    {
		var sq = new SelectQuery();
		var (f, q) = sq.From(source).As("q");
		sq.Select("count(*)").As(alias);
		return sq;
    }
}