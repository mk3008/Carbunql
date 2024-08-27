using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for <see cref="QueryBase"/> and its derived types.
/// </summary>
public static class QueryBaseExtension
{
    /// <summary>
    /// Imports the common table expressions (CTEs) from the specified <see cref="IReadQuery"/> into the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="target">The target <see cref="IReadQuery"/> containing the CTEs to import.</param>
    /// <returns>The <see cref="SelectQuery"/> with the imported CTEs.</returns>
    [Obsolete("With clauses do not need to be manually imported.")]
    public static SelectQuery ImportCommonTable(this SelectQuery source, IReadQuery target)
    {
        var withClauses = target.GetWithClause();
        if (withClauses == null) return source;

        source.WithClause ??= new WithClause();
        foreach (var item in withClauses)
        {
            source.WithClause.Add(item);
        }
        return source;
    }

    /// <summary>
    /// Specifies the source table for the SELECT query.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The source <see cref="CommonTable"/> to include in the FROM clause.</param>
    /// <returns>The constructed FROM clause.</returns>
    public static FromClause From(this SelectQuery source, CommonTable table)
    {
        return source.From(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Specifies the source table for the SELECT query.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The source <see cref="SelectableTable"/> to include in the FROM clause.</param>
    /// <returns>The constructed FROM clause.</returns>
    public static FromClause From(this SelectQuery source, SelectableTable table)
    {
        var fromClause = new FromClause(table);
        source.FromClause = fromClause;
        return fromClause;
    }

    /// <summary>
    /// Specifies the source table for the SELECT query.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The name of the source table to include in the FROM clause.</param>
    /// <returns>The constructed FROM clause.</returns>
    public static FromClause From(this SelectQuery source, string table)
    {
        return source.From(string.Empty, table);
    }

    /// <summary>
    /// Specifies the source table for the SELECT query.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="schema">The schema of the source table.</param>
    /// <param name="table">The name of the source table to include in the FROM clause.</param>
    /// <returns>The constructed FROM clause.</returns>
    public static FromClause From(this SelectQuery source, string schema, string table)
    {
        var selectableTable = new PhysicalTable(schema, table).ToSelectable();
        return source.From(selectableTable);
    }

    public static FromClause From(this SelectQuery source, ITable table)
    {
        var selectableTable = new PhysicalTable(table.Schema, table.Table).ToSelectable();
        return source.From(selectableTable);
    }

    /// <summary>
    /// Specifies the source table for the SELECT query.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="subQuery">The subquery to include in the FROM clause.</param>
    /// <returns>The constructed FROM clause.</returns>
    public static FromClause From(this SelectQuery source, IReadQuery subQuery)
    {
        var virtualTable = new VirtualTable(subQuery);
        var selectableTable = virtualTable.ToSelectable("q");
        return source.From(selectableTable);
    }

    /// <summary>
    /// Specifies the source table for the SELECT query using a builder function.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="builder">A function that returns the subquery to include in the FROM clause.</param>
    /// <returns>The constructed FROM clause.</returns>
    public static FromClause From(this SelectQuery source, Func<IReadQuery> builder)
    {
        return source.From(builder());
    }

    /// <summary>
    /// Specifies a common table expression (CTE) in the SELECT query using a raw SQL string.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="query">The raw SQL string representing the CTE.</param>
    /// <returns>The constructed common table expression.</returns>
    public static CommonTable With(this SelectQuery source, string query)
    {
        var subQuery = SelectQuery.Parse(query);
        return source.With(subQuery.ToCommonTable("cte"));
    }

    /// <summary>
    /// Specifies a common table expression (CTE) in the SELECT query using an <see cref="IReadQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="query">The <see cref="IReadQuery"/> representing the CTE.</param>
    /// <returns>The constructed common table expression.</returns>
    public static CommonTable With(this SelectQuery source, IReadQuery query)
    {
        return source.With(query.ToCommonTable("cte"));
    }

    /// <summary>
    /// Specifies a common table expression (CTE) in the SELECT query using a builder function.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="builder">A function that returns the <see cref="SelectQuery"/> representing the CTE.</param>
    /// <returns>The constructed common table expression.</returns>
    public static CommonTable With(this SelectQuery source, Func<SelectQuery> builder)
    {
        return source.With(builder());
    }

    /// <summary>
    /// Specifies a common table expression (CTE) in the SELECT query.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="ct">The <see cref="CommonTable"/> to include in the WITH clause.</param>
    /// <returns>The constructed common table expression.</returns>
    public static CommonTable With(this SelectQuery source, CommonTable ct)
    {
        source.WithClause ??= new WithClause();
        source.WithClause.Add(ct);
        return ct;
    }

    /// <summary>
    /// Specifies a common table expression (CTE) in the SELECT query using a VALUES query and column aliases.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="q">The <see cref="ValuesQuery"/> representing the CTE.</param>
    /// <param name="columnAliases">The column aliases for the CTE.</param>
    /// <returns>The constructed common table expression.</returns>
    public static CommonTable With(this SelectQuery source, ValuesQuery q, IEnumerable<string> columnAliases)
    {
        return source.With(q.ToCommonTable("cte", columnAliases));
    }

    /// <summary>
    /// Specifies a common table expression (CTE) in the SELECT query using a builder function.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="builder">A function that returns the <see cref="CommonTable"/> representing the CTE.</param>
    /// <returns>The constructed common table expression.</returns>
    public static CommonTable With(this SelectQuery source, Func<CommonTable> builder)
    {
        return source.With(builder());
    }

    /// <summary>
    /// Selects columns from the specified <see cref="FromClause"/> and adds them to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="from">The <see cref="FromClause"/> defining the source table.</param>
    /// <param name="overwrite">If set to <c>true</c>, overwrites existing selections.</param>
    public static void Select(this SelectQuery source, FromClause from, bool overwrite = false)
    {
        source.Select(from.Root);
    }

    /// <summary>
    /// Selects a column from the specified <see cref="FromClause"/> and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="from">The <see cref="FromClause"/> defining the source table.</param>
    /// <param name="column">The name of the column to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified column.</returns>
    public static SelectableItem Select(this SelectQuery source, FromClause from, string column)
    {
        return source.Select(from.Root.Alias, column);
    }

    /// <summary>
    /// Selects a column from the specified <see cref="SelectableTable"/> and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The <see cref="SelectableTable"/> from which to select the column.</param>
    /// <param name="column">The name of the column to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified column.</returns>
    public static SelectableItem Select(this SelectQuery source, SelectableTable table, string column)
    {
        return source.Select(table.Alias, column);
    }


    /// <summary>
    /// Selects all columns from the specified <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of all columns.</returns>
    public static SelectableItem SelectAll(this SelectQuery source)
    {
        var item = new ColumnValue("*").ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects all columns from the specified <see cref="SelectQuery"/> with the specified <see cref="SelectableTable"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The <see cref="SelectableTable"/> from which to select all columns.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of all columns.</returns>
    public static SelectableItem SelectAll(this SelectQuery source, SelectableTable table)
    {
        var item = new ColumnValue(table.Alias, "*").ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects columns from the specified <see cref="SelectableTable"/> and adds them to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The <see cref="SelectableTable"/> from which to select columns.</param>
    /// <param name="overwrite">If set to <c>true</c>, overwrites existing selections.</param>
    public static void Select(this SelectQuery source, SelectableTable table, bool overwrite = false)
    {
        if (!table.GetColumnNames().Any()) throw new Exception("column names is empty.");

        source.SelectClause ??= new();
        var cols = source.GetColumnNames().ToList();
        var newcols = table.GetColumnNames().ToList();

        if (overwrite)
        {
            foreach (var item in cols.Where(x => x.IsEqualNoCase(newcols)))
            {
                var c = source.SelectClause.Where(x => x.Alias == item).First();
                source.SelectClause.Remove(c);
            }
            cols = source.GetColumnNames().ToList();
        }

        foreach (var item in newcols.Where(x => !x.IsEqualNoCase(cols)))
        {
            source.Select(table, item);
        }
    }

    /// <summary>
    /// Selects a column with the specified name from the specified table and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The name of the table containing the column.</param>
    /// <param name="column">The name of the column to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified column.</returns>
    public static SelectableItem Select(this SelectQuery source, string table, string column)
    {
        var item = new ColumnValue(table, column).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects the specified integer value and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="value">The integer value to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified integer value.</returns>
    public static SelectableItem Select(this SelectQuery source, int value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects the specified long integer value and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="value">The long integer value to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified long integer value.</returns>
    public static SelectableItem Select(this SelectQuery source, long value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects the specified decimal value and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="value">The decimal value to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified decimal value.</returns>
    public static SelectableItem Select(this SelectQuery source, decimal value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects the specified double-precision floating point value and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="value">The double-precision floating point value to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified double-precision floating point value.</returns>
    public static SelectableItem Select(this SelectQuery source, double value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects the specified <see cref="DateTime"/> value and adds it to the <see cref="SelectQuery"/> with an optional suffix.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="value">The <see cref="DateTime"/> value to select.</param>
    /// <param name="suffix">An optional suffix to append to the value.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified <see cref="DateTime"/> value.</returns>
    public static SelectableItem Select(this SelectQuery source, DateTime value, string suffix = "::timestamp")
    {
        return source.Select("'" + value.ToString() + "'" + suffix);
    }

    /// <summary>
    /// Selects the specified text and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="text">The text to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified text.</returns>
    public static SelectableItem Select(this SelectQuery source, string text)
    {
        //parse
        var value = ValueParser.Parse(text);
        var item = new SelectableItem(value, "column");
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    /// <summary>
    /// Selects a parameterized value with the specified key and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="key">The key of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the parameterized value.</returns>
    public static SelectableItem SelectParameter(this SelectQuery source, string key, object? value)
    {
        var name = DbmsConfiguration.PlaceholderIdentifier + key;
        source.AddParameter(name, value);

        var item = source.Select(name);
        item.As(key);
        return item;
    }

    /// <summary>
    /// Selects a value returned by the specified builder function and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="builder">The builder function returning the value to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the value returned by the builder function.</returns>
    public static SelectableItem Select(this SelectQuery source, Func<ValueBase> builder)
    {
        return source.Select(builder());
    }

    /// <summary>
    /// Selects the specified value and adds it to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="value">The value to select.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the selection of the specified value.</returns>
    public static SelectableItem Select(this SelectQuery source, ValueBase value)
    {
        var item = value.ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }


    /// <summary>
    /// Orders the result set by the specified column in the root table of the FROM clause.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="from">The FROM clause specifying the root table.</param>
    /// <param name="column">The name of the column to order by.</param>
    public static void Order(this SelectQuery source, FromClause from, string column)
    {
        source.Order(from.Root.Alias, column);
    }

    /// <summary>
    /// Orders the result set by the specified column in the specified table.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The table specifying the column to order by.</param>
    /// <param name="column">The name of the column to order by.</param>
    public static void Order(this SelectQuery source, SelectableTable table, string column)
    {
        source.Order(table.Alias, column);
    }

    /// <summary>
    /// Orders the result set by the specified <see cref="SelectableItem"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="item">The <see cref="SelectableItem"/> representing the column to order by.</param>
    public static void Order(this SelectQuery source, SelectableItem item)
    {
        source.OrderClause ??= new();
        source.OrderClause.Add(item.Value);
    }

    /// <summary>
    /// Orders the result set by the specified column in the specified table.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="table">The name of the table specifying the column to order by.</param>
    /// <param name="column">The name of the column to order by.</param>
    public static void Order(this SelectQuery source, string table, string column)
    {
        var item = new ColumnValue(table, column);
        source.OrderClause ??= new();
        source.OrderClause.Add(item);
    }

    /// <summary>
    /// Adds a named window definition to the <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectQuery"/>.</param>
    /// <param name="namedWindow">The named window definition to add.</param>
    public static void Window(this SelectQuery source, NamedWindowDefinition namedWindow)
    {
        source.WindowClause ??= new();
        source.WindowClause.Add(namedWindow);
    }
}