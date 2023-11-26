using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class RelationExtension
{
	public static Relation As(this Relation source, string Alias)
	{
		source.Table.SetAlias(Alias);
		return source;
	}

	public static SelectableTable On(this Relation source, FromClause from, string column)
	{
		return source.On(from.Root, new[] { column });
	}

	public static SelectableTable On(this Relation source, SelectableTable sourceTable, string column)
	{
		return source.On(sourceTable, new[] { column });
	}

	public static SelectableTable On(this Relation source, SelectableTable sourceTable, IEnumerable<string> columns)
	{
		return source.On(r =>
		{
			ColumnValue? root = null;
			ColumnValue? prev = null;

			foreach (var column in columns)
			{
				var lv = new ColumnValue(sourceTable.Alias, column);
				var rv = new ColumnValue(r.Table.Alias, column);
				lv.AddOperatableValue("=", rv);

				if (prev == null)
				{
					root = lv;
				}
				else
				{
					prev.AddOperatableValue("and", lv);
				}
				prev = rv;
			}

			if (root == null) throw new ArgumentNullException(nameof(columns));
			return root;
		});
	}

	public static SelectableTable On(this Relation source, Func<Relation, ValueBase> builder)
	{
		source.Condition = builder(source);
		return source.Table;
	}

	public static SelectableTable On(this Relation source, Action<Relation> builder)
	{
		builder(source);
		return source.Table;
	}

	public static ValueBase Condition(this Relation source, string table, string column)
	{
		var v = new ColumnValue(table, column);
		return source.Condition(v);
	}

	public static ValueBase Condition(this Relation source, FromClause table, string column)
	{
		var v = new ColumnValue(table, column);
		return source.Condition(v);
	}

	public static ValueBase Condition(this Relation source, SelectableTable table, string column)
	{
		var v = new ColumnValue(table, column);
		return source.Condition(v);
	}

	public static ValueBase Where(this Relation source, string text)
	{
		var v = ValueParser.Parse(text);
		return source.Condition(v);
	}

	public static ValueBase Condition(this Relation source, Func<ValueBase> builder)
	{
		var v = builder();
		return source.Condition(v);
	}

	public static ValueBase Condition(this Relation source, ValueBase value)
	{
		if (source.Condition == null)
		{
			source.Condition = value;
		}
		else
		{
			source.Condition.And(value);
		}
		return value;
	}
}