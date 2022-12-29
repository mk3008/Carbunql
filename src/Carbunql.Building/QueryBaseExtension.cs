using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Building;

public static class QueryBaseExtension
{
    public static SelectableItem ToSelectable(this ColumnValue source)
    {
        return new SelectableItem(source, source.GetDefaultName());
    }

    public static SelectableItem ToSelectable(this LiteralValue source)
    {
        return new SelectableItem(source, "column");
    }

    public static SelectableItem Select(this SelectQuery source, SelectableTable table, string column)
    {
        var item = new ColumnValue(table.Alias, column).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    public static SelectableItem Select(this SelectQuery source, int value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    public static SelectableItem Select(this SelectQuery source, long value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    public static SelectableItem Select(this SelectQuery source, decimal value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    public static SelectableItem Select(this SelectQuery source, double value)
    {
        var item = new LiteralValue(value.ToString()).ToSelectable();
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    public static SelectableItem Select(this SelectQuery source, DateTime value, string sufix = "::timestamp")
    {
        return source.Select("'" + value.ToString() + "'" + sufix);
    }

    public static SelectableItem Select(this SelectQuery source, string text)
    {
        //parse
        var value = ValueParser.Parse(text);
        var item = new SelectableItem(value, "column");
        source.SelectClause ??= new();
        source.SelectClause.Add(item);
        return item;
    }

    public static void As(this SelectableItem source, string alias)
    {
        source.SetAlias(alias);
    }
}
