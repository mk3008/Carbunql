using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Building;

public static class ReadQueryExtension
{
    public static CTEQuery ToCTE(this IReadQuery source, string alias)
    {
        var sq = new CTEQuery();

        sq.WithClause.Add(source.ToCommonTable(alias));

        return sq;
    }

    public static CommonTable ToCommonTable(this IReadQuery source, string alias)
    {
        return new CommonTable(new VirtualTable(source), alias);
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

    public static (SelectQuery, FromClause) ToSubQuery(this IReadQuery source, string alias)
    {
        var sq = new SelectQuery();
        var f = sq.From(source, alias);
        return (sq, f);
    }

    public static SelectQuery ToSubQuery(this IReadQuery source, string alias, Predicate<SelectableItem> columnFilter)
    {
        var s = source.GetSelectClause();
        if (s == null) throw new NotSupportedException();

        var sq = new SelectQuery();
        var f = sq.From(source, alias);
        foreach (var item in s.Items)
        {
            if (!columnFilter(item)) continue;
            sq.Select(f, item.Alias);
        }
        return sq;
    }

    public static CreateTableQuery ToCreateTableQuery(this IReadQuery source, string table)
    {
        var t = table.ToPhysicalTable();
        return source.ToCreateTableQuery(t);
    }

    public static CreateTableQuery ToCreateTableQuery(this IReadQuery source, TableBase table)
    {
        return new CreateTableQuery(new CreateTableClause(table))
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
        if (s == null) throw new NotSupportedException();
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
        var t = table.ToPhysicalTable().ToSelectable();
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
        return source.ToDeleteQuery(t, keys);
    }

    private static WhereClause ToWhereClauseAsDelete(this IReadQuery source, IEnumerable<string> keys, string alias, string queryAlias)
    {
        var ks = keys.ToList();

        var cnd = new ValueCollection();
        ks.ForEach(x => cnd.Add(new ColumnValue(alias, x)));
        if (cnd == null) throw new Exception();

        var sq = source.ToSubQuery(queryAlias, (x) =>
        {
            if (ks.Where(k => k.AreEqual(x.Alias)).Any()) return true;
            return false;
        });
        var exp = new InExpression(cnd.ToBracket(), sq.ToValue());
        return exp.ToWhereClause();
    }

    public static DeleteQuery ToDeleteQuery(this IReadQuery source, SelectableTable table, IEnumerable<string> keys)
    {
        var queryAlias = "q";

        return new DeleteQuery()
        {
            DeleteClause = new DeleteClause(table),
            Parameters = source.GetParameters(),
            WhereClause = source.ToWhereClauseAsDelete(keys, table.Alias, queryAlias),
        };
    }
}