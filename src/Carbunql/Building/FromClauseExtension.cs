using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Building;

public static class FromClauseExtension
{
	public static SelectableTable ToSelectable(this PhysicalTable source)
	{
		return new SelectableTable(source, source.GetDefaultName());
	}

	public static SelectableTable ToSelectable(this VirtualTable source, string alias)
	{
		return new SelectableTable(source, alias);
	}

	public static FromClause From(this SelectQuery source, CommonTable table)
	{
		return source.From(table.ToPhysicalTable().ToSelectable());
	}

	public static FromClause From(this SelectQuery source, SelectableTable table)
	{
		var f = new FromClause(table);
		source.FromClause = f;
		return f;
	}

	public static FromClause From(this SelectQuery source, string table)
	{
		return source.From(string.Empty, table);
	}

	public static FromClause From(this SelectQuery source, string schema, string table)
	{
		var st = new PhysicalTable(schema, table).ToSelectable();
		return source.From(st);
	}

	public static FromClause From(this SelectQuery source, IReadQuery subQuery)
	{
		var vt = new VirtualTable(subQuery);
		var st = vt.ToSelectable("q");
		return source.From(st);
	}

	public static FromClause From(this SelectQuery source, Func<IReadQuery> builder)
	{
		return source.From(builder());
	}

	public static (FromClause, SelectableTable) As(this FromClause source, string alias)
	{
		if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException("alias");
		source.Root.SetAlias(alias);
		return (source, source.Root);
	}

	public static Relation InnerJoin(this FromClause source, IReadQuery query)
	{
		var st = query.ToSelectableTable();
		return source.InnerJoin(st);
	}

	public static Relation InnerJoin(this FromClause source, string table)
	{
		var st = new PhysicalTable(table).ToSelectable();
		return source.InnerJoin(st);
	}

	public static Relation InnerJoin(this FromClause source, string schema, string table)
	{
		var st = new PhysicalTable(schema, table).ToSelectable();
		return source.InnerJoin(st);
	}

	public static Relation InnerJoin(this FromClause source, CommonTable table)
	{
		return source.InnerJoin(table.ToPhysicalTable().ToSelectable());
	}

	public static Relation InnerJoin(this FromClause source, SelectableTable table)
	{
		return source.Join(table, "inner join");
	}

	public static Relation LeftJoin(this FromClause source, IReadQuery query)
	{
		var st = query.ToSelectableTable();
		return source.LeftJoin(st);
	}

	public static Relation LeftJoin(this FromClause source, string table)
	{
		var st = new PhysicalTable(table).ToSelectable();
		return source.LeftJoin(st);
	}

	public static Relation LeftJoin(this FromClause source, string schema, string table)
	{
		var st = new PhysicalTable(schema, table).ToSelectable();
		return source.LeftJoin(st);
	}

	public static Relation LeftJoin(this FromClause source, CommonTable table)
	{
		return source.LeftJoin(table.ToPhysicalTable().ToSelectable());
	}

	public static Relation LeftJoin(this FromClause source, SelectableTable table)
	{
		return source.Join(table, "left join");
	}

	public static Relation RightJoin(this FromClause source, IReadQuery query)
	{
		var st = query.ToSelectableTable();
		return source.RightJoin(st);
	}

	public static Relation RightJoin(this FromClause source, string table)
	{
		var st = new PhysicalTable(table).ToSelectable();
		return source.RightJoin(st);
	}

	public static Relation RightJoin(this FromClause source, string schema, string table)
	{
		var st = new PhysicalTable(schema, table).ToSelectable();
		return source.RightJoin(st);
	}

	public static Relation RightJoin(this FromClause source, CommonTable table)
	{
		return source.RightJoin(table.ToPhysicalTable().ToSelectable());
	}

	public static Relation RightJoin(this FromClause source, SelectableTable table)
	{
		return source.Join(table, "right join");
	}

	public static Relation CrossJoin(this FromClause source, IReadQuery query)
	{
		var st = query.ToSelectableTable();
		return source.CrossJoin(st);
	}

	public static Relation CrossJoin(this FromClause source, string table)
	{
		var st = new PhysicalTable(table).ToSelectable();
		return source.CrossJoin(st);
	}

	public static Relation CrossJoin(this FromClause source, string schema, string table)
	{
		var st = new PhysicalTable(schema, table).ToSelectable();
		return source.CrossJoin(st);
	}

	public static Relation CrossJoin(this FromClause source, CommonTable table)
	{
		return source.CrossJoin(table.ToPhysicalTable().ToSelectable());
	}

	public static Relation CrossJoin(this FromClause source, SelectableTable table)
	{
		return source.Join(table, "cross join");
	}

	public static Relation Join(this FromClause source, string schema, string table, string join)
	{
		var st = new PhysicalTable(schema, table).ToSelectable();
		return source.Join(st, join);
	}

	public static Relation Join(this FromClause source, SelectableTable table, string join)
	{
		var r = new Relation(table, join);

		source.Relations ??= new();
		source.Relations.Add(r);
		return r;
	}

	public static Relation As(this Relation source, string Alias)
	{
		source.Table.SetAlias(Alias);
		return source;
	}

	public static (Relation, T) As<T>(this Relation source, string Alias)
	{
		source.Table.SetAlias(Alias);
		var r = (T)Activator.CreateInstance(typeof(T))!;
		return (source, r);
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
}