using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Extension methods for the <see cref="SelectClause"/> class.
/// </summary>
public static class SelectClauseExtension
{
    /// <summary>
    /// Converts a <see cref="ColumnValue"/> to a <see cref="SelectableItem"/>.
    /// </summary>
    /// <param name="source">The source <see cref="ColumnValue"/>.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the column.</returns>
    public static SelectableItem ToSelectable(this ColumnValue source)
    {
        return new SelectableItem(source, source.GetDefaultName());
    }

    /// <summary>
    /// Converts a <see cref="ValueBase"/> to a <see cref="SelectableItem"/> with a specified name.
    /// </summary>
    /// <param name="source">The source <see cref="ValueBase"/>.</param>
    /// <param name="name">The name of the selectable item.</param>
    /// <returns>The <see cref="SelectableItem"/> representing the value.</returns>
    public static SelectableItem ToSelectable(this ValueBase source, string name = "column")
    {
        return new SelectableItem(source, name);
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
    }/// <summary>
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
    /// Sets the alias for the <see cref="SelectableItem"/>.
    /// </summary>
    /// <param name="source">The source <see cref="SelectableItem"/>.</param>
    /// <param name="alias">The alias to set.</param>
    public static void As(this SelectableItem source, string alias)
    {
        source.SetAlias(alias);
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
