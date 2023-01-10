using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;

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

    public static FromClause From(this SelectQuery source, string table)
    {
        return source.From(string.Empty, table);
    }

    public static FromClause From(this SelectQuery source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        var f = new FromClause(st);
        source.FromClause = f;
        return f;
    }

    public static FromClause From(this SelectQuery source, IReadQuery subQuery, string alias)
    {
        var vt = new VirtualTable(subQuery);
        var st = vt.ToSelectable(alias);
        var f = new FromClause(st);
        source.FromClause = f;
        return f;
    }

    public static FromClause As(this FromClause source, string alias)
    {
        source.Root.SetAlias(alias);
        return source;
    }

    public static Relation InnerJoin(this FromClause source, string table)
    {
        return source.InnerJoin(source.Root, string.Empty, table);
    }

    public static Relation InnerJoin(this FromClause source, string schema, string table)
    {
        return source.InnerJoin(source.Root, schema, table);
    }

    public static Relation InnerJoin(this FromClause source, SelectableTable sourceTable, string schema, string table)
    {
        return source.Join(schema, table, TableJoin.Inner);
    }

    public static Relation LeftJoin(this FromClause source, string table)
    {
        return source.LeftJoin(source.Root, string.Empty, table);
    }

    public static Relation LeftJoin(this FromClause source, string schema, string table)
    {
        return source.LeftJoin(source.Root, schema, table);
    }

    public static Relation LeftJoin(this FromClause source, SelectableTable sourceTable, string schema, string table)
    {
        return source.Join(schema, table, TableJoin.Left);
    }

    public static Relation RightJoin(this FromClause source, string table)
    {
        return source.RightJoin(source.Root, string.Empty, table);
    }

    public static Relation RightJoin(this FromClause source, string schema, string table)
    {
        return source.RightJoin(source.Root, schema, table);
    }

    public static Relation RightJoin(this FromClause source, SelectableTable sourceTable, string schema, string table)
    {
        return source.Join(schema, table, TableJoin.Right);
    }

    public static Relation CrossJoin(this FromClause source, string table)
    {
        return source.CrossJoin(source.Root, string.Empty, table);
    }

    public static Relation CrossJoin(this FromClause source, string schema, string table)
    {
        return source.CrossJoin(source.Root, schema, table);
    }

    public static Relation CrossJoin(this FromClause source, SelectableTable sourceTable, string schema, string table)
    {
        return source.Join(schema, table, TableJoin.Cross);
    }

    public static Relation Join(this FromClause source, string schema, string table, TableJoin join)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        var r = new Relation(st, join);

        source.Relations ??= new();
        source.Relations.Add(r);
        return r;
    }

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
}