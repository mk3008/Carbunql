using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Fluent;

public static class SelectQueryConvertExtension
{
    /// <summary>
    /// Returns a select query that references itself as a CTE (Common Table Expression).
    /// </summary>
    /// <param name="name">The name of the CTE.</param>
    /// <param name="alias">
    /// The alias to use. If omitted, the select clause will use a wildcard.
    /// </param>
    /// <returns>The modified select query with the CTE reference.</returns>
    /// <exception cref="InvalidProgramException">
    /// Thrown if a CTE with the same name already exists.
    /// </exception>
    public static SelectQuery ToCteQuery(this SelectQuery source, string name, string alias)
    {
        return source.ToCteQuery(name, alias, out _);
    }

    /// <summary>
    /// Returns a select query that references itself as a CTE (Common Table Expression).
    /// </summary>
    /// <param name="name">The name of the CTE.</param>
    /// <param name="alias">
    /// The alias to use. If omitted, the select clause will use a wildcard.
    /// </param>
    /// <returns>The modified select query with the CTE reference.</returns>
    /// <exception cref="InvalidProgramException">
    /// Thrown if a CTE with the same name already exists.
    /// </exception>
    public static SelectQuery ToCteQuery(this SelectQuery source, string name, string alias, out FluentTable table)
    {
        if (source.GetCommonTables().Where(x => x.Alias.IsEqualNoCase(name)).Any())
        {
            throw new InvalidProgramException($"A CTE with the same name already exists. Name: {name}");
        }

        table = FluentTable.Create(source, name, alias);

        var sq = new SelectQuery()
            .From(table);

        return sq;
    }

    public static SelectQuery ToSubQuery(this SelectQuery source, string alias, out FluentTable table)
    {
        table = FluentTable.Create(source, alias);

        var sq = new SelectQuery()
            .From(table);

        return sq;
    }

    /// <summary>
    /// Converts the SelectQuery to a CreateTableQuery with the specified table name and optional flag for temporary table.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The name of the table.</param>
    /// <param name="isTemporary">A flag indicating whether the table is temporary (default is true).</param>
    /// <returns>The CreateTableQuery representing the SelectQuery with the specified table name and temporary flag.</returns>
    public static CreateTableQuery ToCreateTableQuery(this SelectQuery source, string table, bool isTemporary = true)
    {
        var t = table.ToPhysicalTable();
        return source.ToCreateTableQuery(t, isTemporary);
    }

    /// <summary>
    /// Converts the SelectQuery to a CreateTableQuery with the specified TableBase and optional flag for temporary table.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The TableBase representing the table.</param>
    /// <param name="isTemporary">A flag indicating whether the table is temporary (default is true).</param>
    /// <returns>The CreateTableQuery representing the SelectQuery with the specified TableBase and temporary flag.</returns>
    public static CreateTableQuery ToCreateTableQuery(this SelectQuery source, TableBase table, bool isTemporary)
    {
        return new CreateTableQuery(table.GetTableFullName())
        {
            IsTemporary = isTemporary,
            Parameters = source.GetParameters(),
            Query = source,
        };
    }

    /// <summary>
    /// Converts the SelectQuery to an InsertQuery for insertion into the specified table.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The name of the table for insertion.</param>
    /// <returns>The InsertQuery representing the SelectQuery for insertion into the specified table.</returns>
    public static InsertQuery ToInsertQuery(this SelectQuery source, string table)
    {
        var t = source.ToInsertTable(table);
        return source.ToInsertQuery(t);
    }

    /// <summary>
    /// Converts the SelectQuery to an InsertQuery for insertion into the specified SelectableTable.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The SelectableTable for insertion.</param>
    /// <returns>The InsertQuery representing the SelectQuery for insertion into the specified SelectableTable.</returns>
    public static InsertQuery ToInsertQuery(this SelectQuery source, SelectableTable table)
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
    /// Converts the SelectQuery to an UpdateQuery for updating rows in the specified table and alias using the provided keys.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The name of the table for update.</param>
    /// <param name="alias">The alias for the table.</param>
    /// <param name="keys">The keys to use for the update operation.</param>
    /// <returns>The UpdateQuery representing the SelectQuery for updating rows in the specified table and alias.</returns>
    public static UpdateQuery ToUpdateQuery(this SelectQuery source, string table, string alias, IEnumerable<string> keys)
    {
        var t = table.ToPhysicalTable().ToSelectable(alias);
        return source.ToUpdateQuery(t, keys);
    }

    /// <summary>
    /// Converts the SelectQuery to an UpdateQuery for updating rows in the specified SelectableTable using the provided keys.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The SelectableTable for update.</param>
    /// <param name="keys">The keys to use for the update operation.</param>
    /// <returns>The UpdateQuery representing the SelectQuery for updating rows in the specified SelectableTable.</returns>
    public static UpdateQuery ToUpdateQuery(this SelectQuery source, SelectableTable table, IEnumerable<string> keys)
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
    private static SetClause ToSetClause(this SelectQuery source, IEnumerable<string> keys, string queryAlias)
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
    /// Converts the SelectQuery to a DeleteQuery for deleting rows in the specified table using the provided keys.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The name of the table for delete.</param>
    /// <param name="keys">The keys to use for the delete operation.</param>
    /// <returns>The DeleteQuery representing the SelectQuery for deleting rows in the specified table.</returns>
    public static DeleteQuery ToDeleteQuery(this SelectQuery source, string table, IEnumerable<string> keys)
    {
        var t = table.ToPhysicalTable().ToSelectable("d");
        return source.ToDeleteQuery(t, keys, source.GetWithClause());
    }

    /// <summary>
    /// Converts the SelectQuery to a DeleteQuery for deleting rows in the specified table.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The name of the table for delete.</param>
    /// <returns>The DeleteQuery representing the SelectQuery for deleting rows in the specified table.</returns>
    public static DeleteQuery ToDeleteQuery(this SelectQuery source, string table)
    {
        var _ = source.GetSelectClause() ?? throw new NullReferenceException("Missing select clause in query.");
        var t = table.ToPhysicalTable().ToSelectable("d");
        return source.ToDeleteQuery(t, source.GetWithClause());
    }

    /// <summary>
    /// Converts the SelectQuery to a DeleteQuery for deleting rows in the specified SelectableTable using the provided keys and optional WithClause.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The SelectableTable for delete.</param>
    /// <param name="keys">The keys to use for the delete operation.</param>
    /// <param name="wclause">Optional WithClause for the delete operation.</param>
    /// <returns>The DeleteQuery representing the SelectQuery for deleting rows in the specified SelectableTable.</returns>
    public static DeleteQuery ToDeleteQuery(this SelectQuery source, SelectableTable table, IEnumerable<string> keys, WithClause? wclause = null)
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
    /// Converts the SelectQuery to a DeleteQuery for deleting rows in the specified SelectableTable with optional WithClause.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="table">The SelectableTable for delete.</param>
    /// <param name="wclause">Optional WithClause for the delete operation.</param>
    /// <returns>The DeleteQuery representing the SelectQuery for deleting rows in the specified SelectableTable.</returns>
    public static DeleteQuery ToDeleteQuery(this SelectQuery source, SelectableTable table, WithClause? wclause = null)
    {
        return new DeleteQuery()
        {
            DeleteClause = new DeleteClause(table),
            Parameters = source.GetParameters(),
            WithClause = wclause,
            WhereClause = source.ToWhereClauseAsDelete(table.Alias),
        };
    }

    private static WhereClause ToWhereClauseAsDelete(this SelectQuery source, IEnumerable<string> keys, string alias, string queryAlias)
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

    private static WhereClause ToWhereClauseAsDelete(this SelectQuery source, string alias)
    {
        var select = source.GetSelectClause();
        var selectColumns = select!.Select(x => x.Alias);

        if (selectColumns == null || !selectColumns.Any()) throw new InvalidOperationException("Missing select clause.");

        var cnd = new ValueCollection(alias, selectColumns);
        var exp = new InClause(cnd.ToBracket(), source.ToValue());
        return exp.ToWhereClause();
    }

    /// <summary>
    /// Converts the SelectQuery to a SelectQuery representing a count query, optionally specifying an alias for the count.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="alias">Optional alias for the count column.</param>
    /// <returns>The SelectQuery representing the count query.</returns>
    public static SelectQuery ToCountQuery(this SelectQuery source, string alias = "row_count")
    {
        var sq = new SelectQuery();
        sq.From(source).As("q");
        sq.Select("count(*)").As(alias);
        return sq;
    }

    /// <summary>
    /// Converts the SelectQuery to a MergeQuery for performing a merge operation on the specified destination table using the provided keys and datasource alias.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="destinationTable">The name of the destination table for the merge operation.</param>
    /// <param name="keys">The keys to use for the merge operation.</param>
    /// <param name="datasourceAlias">The alias for the datasource.</param>
    /// <returns>The MergeQuery representing the merge operation.</returns>
    public static MergeQuery ToMergeQuery(this SelectQuery source, string destinationTable, IEnumerable<string> keys, string datasourceAlias = "s")
    {
        return new MergeQuery(source, destinationTable, keys, datasourceAlias);
    }

    /// <summary>
    /// Converts the SelectQuery to a MergeQuery for performing a merge operation on the specified destination SelectableTable using the provided keys and datasource alias.
    /// </summary>
    /// <param name="source">The source SelectQuery.</param>
    /// <param name="destination">The destination SelectableTable for the merge operation.</param>
    /// <param name="keys">The keys to use for the merge operation.</param>
    /// <param name="datasourceAlias">The alias for the datasource.</param>
    /// <returns>The MergeQuery representing the merge operation.</returns>
    public static MergeQuery ToMergeQuery(this SelectQuery source, SelectableTable destination, IEnumerable<string> keys, string datasourceAlias = "s")
    {
        return new MergeQuery(source, destination, keys, datasourceAlias);
    }
}
