using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for IReadQuery objects.
/// </summary>
public static class ReadQueryExtension
{
    /// <summary>
    /// Converts the IReadQuery to a CommonTableExpression (CTE) with the specified alias.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="alias">The alias for the CTE.</param>
    /// <returns>A tuple containing the SelectQuery and the CommonTable representing the CTE.</returns>
    public static (SelectQuery, CommonTable) ToCTE(this IReadQuery source, string alias)
    {
        var sq = new SelectQuery();

        sq.WithClause ??= new WithClause();
        var ct = source.ToCommonTable(alias);
        sq.WithClause.Add(ct);

        return (sq, ct);
    }

    /// <summary>
    /// Converts the IReadQuery to a CommonTable with the specified alias.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="alias">The alias for the CommonTable.</param>
    /// <returns>The CommonTable representing the IReadQuery with the specified alias.</returns>
    public static CommonTable ToCommonTable(this IReadQuery source, string alias)
    {
        return new CommonTable(new VirtualTable(source), alias);
    }

    /// <summary>
    /// Converts the IReadQuery to a CommonTable with the specified alias and column aliases.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="alias">The alias for the CommonTable.</param>
    /// <param name="columnAliases">The column aliases for the CommonTable.</param>
    /// <returns>The CommonTable representing the IReadQuery with the specified alias and column aliases.</returns>
    public static CommonTable ToCommonTable(this IReadQuery source, string alias, IEnumerable<string> columnAliases)
    {
        return new CommonTable(new VirtualTable(source), alias, columnAliases.ToValueCollection());
    }

    /// <summary>
    /// Converts the IReadQuery to a SelectableTable with the default alias "t".
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <returns>The SelectableTable representing the IReadQuery with the default alias.</returns>
    public static SelectableTable ToSelectableTable(this IReadQuery source)
    {
        return source.ToSelectableTable("t");
    }

    /// <summary>
    /// Converts the IReadQuery to a SelectableTable with the specified alias.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="alias">The alias for the SelectableTable.</param>
    /// <returns>The SelectableTable representing the IReadQuery with the specified alias.</returns>
    public static SelectableTable ToSelectableTable(this IReadQuery source, string alias)
    {
        var t = new VirtualTable(source);
        return new SelectableTable(t, alias);
    }

    /// <summary>
    /// Converts the IReadQuery to a FromClause with the specified alias.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="alias">The alias for the FromClause.</param>
    /// <returns>The FromClause representing the IReadQuery with the specified alias.</returns>
    public static FromClause ToFromClause(this IReadQuery source, string alias)
    {
        var t = source.ToSelectableTable(alias);
        return new FromClause(t);
    }

    /// <summary>
    /// Converts the IReadQuery to an ExistsExpression.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <returns>An ExistsExpression representing the IReadQuery.</returns>
    public static ExistsExpression ToExists(this IReadQuery source)
    {
        return new ExistsExpression(source);
    }

    /// <summary>
    /// Converts the IReadQuery to a NegativeValue representing a NOT EXISTS expression.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <returns>A NegativeValue representing a NOT EXISTS expression for the IReadQuery.</returns>
    public static NegativeValue ToNotExists(this IReadQuery source)
    {
        return new NegativeValue(source.ToExists());
    }

    /// <summary>
    /// Converts the IReadQuery to a subquery with the default alias "q".
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <returns>A subquery representing the IReadQuery with the default alias.</returns>
    public static SelectQuery ToSubQuery(this IReadQuery source)
    {
        return source.ToSubQuery("q");
    }

    /// <summary>
    /// Converts the IReadQuery to a subquery with the specified alias.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="alias">The alias for the subquery.</param>
    /// <returns>A subquery representing the IReadQuery with the specified alias.</returns>
    public static SelectQuery ToSubQuery(this IReadQuery source, string alias)
    {
        var sq = new SelectQuery();
        sq.From(source).As(alias);
        return sq;
    }

    /// <summary>
    /// Converts the IReadQuery to a CreateTableQuery with the specified table name and optional flag for temporary table.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table.</param>
    /// <param name="isTemporary">A flag indicating whether the table is temporary (default is true).</param>
    /// <returns>The CreateTableQuery representing the IReadQuery with the specified table name and temporary flag.</returns>
    public static CreateTableQuery ToCreateTableQuery(this IReadQuery source, string table, bool isTemporary = true)
    {
        var t = table.ToPhysicalTable();
        return source.ToCreateTableQuery(t, isTemporary);
    }

    /// <summary>
    /// Converts the IReadQuery to a CreateTableQuery with the specified TableBase and optional flag for temporary table.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The TableBase representing the table.</param>
    /// <param name="isTemporary">A flag indicating whether the table is temporary (default is true).</param>
    /// <returns>The CreateTableQuery representing the IReadQuery with the specified TableBase and temporary flag.</returns>
    public static CreateTableQuery ToCreateTableQuery(this IReadQuery source, TableBase table, bool isTemporary)
    {
        return new CreateTableQuery(table.GetTableFullName())
        {
            IsTemporary = isTemporary,
            Parameters = source.GetParameters(),
            Query = source,
        };
    }

    /// <summary>
    /// Converts the IReadQuery to a SelectableTable for insertion into the specified table.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table for insertion.</param>
    /// <returns>The SelectableTable representing the IReadQuery for insertion into the specified table.</returns>
    public static SelectableTable ToInsertTable(this IReadQuery source, string table)
    {
        var clause = source.GetSelectClause();
        if (clause == null)
        {
            return new SelectableTable(new PhysicalTable(table), table);
        }

        var vals = new ValueCollection();
        foreach (var item in clause)
        {
            vals.Add(new ColumnValue(item.Alias));
        }

        return new SelectableTable(new PhysicalTable(table), table, vals);
    }

    /// <summary>
    /// Converts the IReadQuery to an InsertQuery for insertion into the specified table.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table for insertion.</param>
    /// <returns>The InsertQuery representing the IReadQuery for insertion into the specified table.</returns>
    public static InsertQuery ToInsertQuery(this IReadQuery source, string table)
    {
        var t = source.ToInsertTable(table);
        return source.ToInsertQuery(t);
    }

    /// <summary>
    /// Converts the IReadQuery to an InsertQuery for insertion into the specified SelectableTable.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The SelectableTable for insertion.</param>
    /// <returns>The InsertQuery representing the IReadQuery for insertion into the specified SelectableTable.</returns>
    public static InsertQuery ToInsertQuery(this IReadQuery source, SelectableTable table)
    {
        if (table.Table is PhysicalTable t)
        {
            var aliases = GetColumnAliasesOrDefault(table);
            return new InsertQuery()
            {
                InsertClause = new InsertClause(t) { ColumnAliases = aliases },
                Parameters = source.GetParameters(),
                Query = source,
            };
        }
        throw new InvalidCastException("Only physical tables can be converted to insert query.");
    }

    private static ValueCollection? GetColumnAliasesOrDefault(SelectableTable table)
    {
        if (table.ColumnAliases != null && table.ColumnAliases.Any())
        {
            return table.ColumnAliases;
        }
        return null;
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

    /// <summary>
    /// Converts the IReadQuery to an UpdateQuery for updating rows in the specified table using the provided keys.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table for update.</param>
    /// <param name="keys">The keys to use for the update operation.</param>
    /// <returns>The UpdateQuery representing the IReadQuery for updating rows in the specified table.</returns>
    public static UpdateQuery ToUpdateQuery(this IReadQuery source, string table, IEnumerable<string> keys)
    {
        var t = table.ToPhysicalTable().ToSelectable("d");
        return source.ToUpdateQuery(t, keys);
    }

    /// <summary>
    /// Converts the IReadQuery to an UpdateQuery for updating rows in the specified table and alias using the provided keys.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table for update.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="keys">The keys to use for the update operation.</param>
    /// <returns>The UpdateQuery representing the IReadQuery for updating rows in the specified table and alias.</returns>
    public static UpdateQuery ToUpdateQuery(this IReadQuery source, string table, string alias, IEnumerable<string> keys)
    {
        var t = table.ToPhysicalTable().ToSelectable(alias);
        return source.ToUpdateQuery(t, keys);
    }

    /// <summary>
    /// Converts the IReadQuery to an UpdateQuery for updating rows in the specified SelectableTable using the provided keys.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The SelectableTable for update.</param>
    /// <param name="keys">The keys to use for the update operation.</param>
    /// <returns>The UpdateQuery representing the IReadQuery for updating rows in the specified SelectableTable.</returns>
    public static UpdateQuery ToUpdateQuery(this IReadQuery source, SelectableTable table, IEnumerable<string> keys)
    {
        var queryAlias = "q";

        return new UpdateQuery()
        {
            WithClause = source.GetWithClause(),
            UpdateClause = new UpdateClause(table),
            SetClause = source.ToSetClause(keys, queryAlias),
            Parameters = source.GetParameters(),
            FromClause = source.ToFromClause(queryAlias),
            WhereClause = ToWhereClauseAsUpdate(keys, table.Alias, queryAlias),
        };
    }

    /// <summary>
    /// set val = queryAlias.val
    /// </summary>
    /// <param name="source"></param>
    /// <param name="keys"></param>
    /// <param name="queryAlias"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private static SetClause ToSetClause(this IReadQuery source, IEnumerable<string> keys, string queryAlias)
    {
        var selectclause = source.GetSelectClause() ?? throw new NotSupportedException("select clause is not found.");
        var cols = selectclause.Where(x => !keys.Contains(x.Alias)).Select(x => x.Alias).ToList();

        var clause = new SetClause();
        foreach (var item in cols)
        {
            var c = new ColumnValue(item);
            c.Equal(new ColumnValue(queryAlias, item));
            clause.Add(c);
        };
        return clause;
    }

    /// <summary>
    /// Converts the IReadQuery to a DeleteQuery for deleting rows in the specified table using the provided keys.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table for delete.</param>
    /// <param name="keys">The keys to use for the delete operation.</param>
    /// <returns>The DeleteQuery representing the IReadQuery for deleting rows in the specified table.</returns>
    public static DeleteQuery ToDeleteQuery(this IReadQuery source, string table, IEnumerable<string> keys)
    {
        var t = table.ToPhysicalTable().ToSelectable("d");
        return source.ToDeleteQuery(t, keys, source.GetWithClause());
    }

    /// <summary>
    /// Converts the IReadQuery to a DeleteQuery for deleting rows in the specified table.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The name of the table for delete.</param>
    /// <returns>The DeleteQuery representing the IReadQuery for deleting rows in the specified table.</returns>
    public static DeleteQuery ToDeleteQuery(this IReadQuery source, string table)
    {
        var _ = source.GetSelectClause() ?? throw new NullReferenceException("Missing select clause in query.");
        var t = table.ToPhysicalTable().ToSelectable("d");
        return source.ToDeleteQuery(t, source.GetWithClause());
    }

    /// <summary>
    /// Converts the IReadQuery to a DeleteQuery for deleting rows in the specified SelectableTable using the provided keys and optional WithClause.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The SelectableTable for delete.</param>
    /// <param name="keys">The keys to use for the delete operation.</param>
    /// <param name="wclause">Optional WithClause for the delete operation.</param>
    /// <returns>The DeleteQuery representing the IReadQuery for deleting rows in the specified SelectableTable.</returns>
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

    /// <summary>
    /// Converts the IReadQuery to a DeleteQuery for deleting rows in the specified SelectableTable with optional WithClause.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="table">The SelectableTable for delete.</param>
    /// <param name="wclause">Optional WithClause for the delete operation.</param>
    /// <returns>The DeleteQuery representing the IReadQuery for deleting rows in the specified SelectableTable.</returns>
    public static DeleteQuery ToDeleteQuery(this IReadQuery source, SelectableTable table, WithClause? wclause = null)
    {
        return new DeleteQuery()
        {
            DeleteClause = new DeleteClause(table),
            Parameters = source.GetParameters(),
            WithClause = wclause,
            WhereClause = source.ToWhereClauseAsDelete(table.Alias),
        };
    }

    private static WhereClause ToWhereClauseAsDelete(this IReadQuery source, IEnumerable<string> keys, string alias, string queryAlias)
    {
        var ks = keys.ToList();

        var cnd = new ValueCollection(alias, keys);

        var sq = new SelectQuery();
        var (f, a) = sq.From(source).As(queryAlias);
        foreach (var item in a.Table.GetColumnNames())
        {
            if (!ks.Where(k => k.IsEqualNoCase(item)).Any()) continue;
            sq.Select(a, item);
        }

        var exp = new InClause(cnd.ToBracket(), sq.ToValue());
        return exp.ToWhereClause();
    }

    private static WhereClause ToWhereClauseAsDelete(this IReadQuery source, string alias)
    {
        var select = source.GetSelectClause();
        var selectColumns = select!.Select(x => x.Alias);

        if (selectColumns == null || !selectColumns.Any()) throw new InvalidOperationException("Missing select clause.");

        var cnd = new ValueCollection(alias, selectColumns);
        var exp = new InClause(cnd.ToBracket(), source.ToValue());
        return exp.ToWhereClause();
    }

    /// <summary>
    /// Performs a union all operation with another IReadQuery and modifies the source IReadQuery accordingly.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="query">The other IReadQuery to union all with.</param>
    public static void UnionAll(this IReadQuery source, IReadQuery query)
    {
        var sq = source.GetOrNewSelectQuery();
        sq.AddOperatableValue("union all", query);
    }

    /// <summary>
    /// Performs a union all operation with a dynamically created IReadQuery using the provided builder function and modifies the source IReadQuery accordingly.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="builder">The builder function to dynamically create the IReadQuery to union all with.</param>
    public static void UnionAll(this IReadQuery source, Func<IReadQuery> builder)
    {
        source.UnionAll(builder());
    }

    /// <summary>
    /// Performs a union operation with another IReadQuery and modifies the source IReadQuery accordingly.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="query">The other IReadQuery to union with.</param>
    public static void Union(this IReadQuery source, IReadQuery query)
    {
        var sq = source.GetOrNewSelectQuery();
        sq.AddOperatableValue("union", query);
    }

    /// <summary>
    /// Performs a union operation with a dynamically created IReadQuery using the provided builder function and modifies the source IReadQuery accordingly.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="builder">The builder function to dynamically create the IReadQuery to union with.</param>
    public static void Union(this IReadQuery source, Func<IReadQuery> builder)
    {
        source.Union(builder());
    }

    /// <summary>
    /// Converts the IReadQuery to a SelectQuery representing a count query, optionally specifying an alias for the count.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="alias">Optional alias for the count column.</param>
    /// <returns>The SelectQuery representing the count query.</returns>
    public static SelectQuery ToCountQuery(this IReadQuery source, string alias = "row_count")
    {
        var sq = new SelectQuery();
        sq.From(source).As("q");
        sq.Select("count(*)").As(alias);
        return sq;
    }

    /// <summary>
    /// Converts the IReadQuery to a MergeQuery for performing a merge operation on the specified destination table using the provided keys and datasource alias.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="destinationTable">The name of the destination table for the merge operation.</param>
    /// <param name="keys">The keys to use for the merge operation.</param>
    /// <param name="datasourceAlias">The alias for the datasource.</param>
    /// <returns>The MergeQuery representing the merge operation.</returns>
    public static MergeQuery ToMergeQuery(this IReadQuery source, string destinationTable, IEnumerable<string> keys, string datasourceAlias = "s")
    {
        return new MergeQuery(source, destinationTable, keys, datasourceAlias);
    }

    /// <summary>
    /// Converts the IReadQuery to a MergeQuery for performing a merge operation on the specified destination SelectableTable using the provided keys and datasource alias.
    /// </summary>
    /// <param name="source">The source IReadQuery.</param>
    /// <param name="destination">The destination SelectableTable for the merge operation.</param>
    /// <param name="keys">The keys to use for the merge operation.</param>
    /// <param name="datasourceAlias">The alias for the datasource.</param>
    /// <returns>The MergeQuery representing the merge operation.</returns>
    public static MergeQuery ToMergeQuery(this IReadQuery source, SelectableTable destination, IEnumerable<string> keys, string datasourceAlias = "s")
    {
        return new MergeQuery(source, destination, keys, datasourceAlias);
    }
}
