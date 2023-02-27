using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Building;

public static class SelectClauseExtension
{
	public static SelectableItem ToSelectable(this ColumnValue source)
	{
		return new SelectableItem(source, source.GetDefaultName());
	}

	public static SelectableItem ToSelectable(this ValueBase source, string name = "column")
	{
		return new SelectableItem(source, name);
	}

	public static SelectableItem SelectAll(this SelectQuery source)
	{
		var item = new ColumnValue("*").ToSelectable();
		source.SelectClause ??= new();
		source.SelectClause.Add(item);
		return item;
	}

	public static SelectableItem SelectAll(this SelectQuery source, FromClause from)
	{
		return source.SelectAll(from.Root);
	}

	public static SelectableItem SelectAll(this SelectQuery source, SelectableTable table)
	{
		var item = new ColumnValue(table.Alias, "*").ToSelectable();
		source.SelectClause ??= new();
		source.SelectClause.Add(item);
		return item;
	}

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

	public static void Select(this SelectQuery source, FromClause from, bool overwrite = false)
	{
		source.Select(from.Root);
	}

	public static SelectableItem Select(this SelectQuery source, FromClause from, string column)
	{
		return source.Select(from.Root.Alias, column);
	}

	public static SelectableItem Select(this SelectQuery source, SelectableTable table, string column)
	{
		return source.Select(table.Alias, column);
	}

	public static SelectableItem Select(this SelectQuery source, string table, string column)
	{
		var item = new ColumnValue(table, column).ToSelectable();
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

	public static SelectableItem Select(this SelectQuery source, Func<ValueBase> builder)
	{
		return source.Select(builder());
	}

	public static SelectableItem Select(this SelectQuery source, ValueBase value)
	{
		var item = value.ToSelectable();
		source.SelectClause ??= new();
		source.SelectClause.Add(item);
		return item;
	}

	public static void As(this SelectableItem source, string alias)
	{
		source.SetAlias(alias);
	}

	public static void Order(this SelectQuery source, FromClause from, string column)
	{
		source.Order(from.Root.Alias, column);
	}

	public static void Order(this SelectQuery source, SelectableTable table, string column)
	{
		source.Order(table.Alias, column);
	}

	public static void Order(this SelectQuery source, string table, string column)
	{
		var item = new ColumnValue(table, column);
		source.Order(item);
	}

	public static void Order(this SelectQuery source, IQueryCommandable value)
	{
		source.OrderClause ??= new OrderClause();
		source.OrderClause.Add(value);
	}
}