using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class WhereClauseExtension
{
	public static void Where(this SelectQuery source, Func<ValueBase> builder)
	{
		if (source.WhereClause == null)
		{
			source.WhereClause = new WhereClause(builder());
		}
		else
		{
			var v = source.WhereClause.Condition.GetLast();
			v.And(builder());
		}
	}

	public static void Where(this SelectQuery source, ValueBase value)
	{
		if (source.WhereClause == null)
		{
			source.WhereClause = new WhereClause(value);
		}
		else
		{
			var v = source.WhereClause.Condition.GetLast();
			v.And(value);
		}
	}

	public static ColumnValue WhereColumn(this SelectQuery source, string column)
	{
		var c = new ColumnValue(column);
		source.Where(c);
		return c;
	}

	public static ColumnValue WhereColumn(this SelectQuery source, string table, string column)
	{
		var c = new ColumnValue(table, column);
		source.Where(c);
		return c;
	}

	public static ColumnValue WhereColumn(this SelectQuery source, SelectableTable table, string column)
	{
		var c = new ColumnValue(table, column);
		source.Where(c);
		return c;
	}
}