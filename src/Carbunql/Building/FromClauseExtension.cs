using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for building SQL FROM clauses.
/// </summary>
public static class FromClauseExtension
{
    /// <summary>
    /// Sets an alias for the source table.
    /// </summary>
    public static (FromClause, SelectableTable) As(this FromClause source, string alias)
    {
        if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException("alias");
        source.Root.SetAlias(alias);
        return (source, source.Root);
    }

    /// <summary>
    /// Performs an inner join with the specified query.
    /// </summary>
    public static Relation InnerJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.InnerJoin(st);
    }

    /// <summary>
    /// Performs an inner join with the specified table name.
    /// </summary>
    public static Relation InnerJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.InnerJoin(st);
    }

    /// <summary>
    /// Performs an inner join with the specified schema and table name.
    /// </summary>
    public static Relation InnerJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.InnerJoin(st);
    }

    /// <summary>
    /// Performs an inner join with the specified common table.
    /// </summary>
    public static Relation InnerJoin(this FromClause source, CommonTable table)
    {
        return source.InnerJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs an inner join with the specified selectable table.
    /// </summary>
    public static Relation InnerJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "inner join");
    }


    /// <summary>
    /// Performs a left join with the specified query.
    /// </summary>
    public static Relation LeftJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.LeftJoin(st);
    }

    /// <summary>
    /// Performs a left join with the specified table name.
    /// </summary>
    public static Relation LeftJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.LeftJoin(st);
    }

    /// <summary>
    /// Performs a left join with the specified schema and table name.
    /// </summary>
    public static Relation LeftJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.LeftJoin(st);
    }

    /// <summary>
    /// Performs a left join with the specified common table.
    /// </summary>
    public static Relation LeftJoin(this FromClause source, CommonTable table)
    {
        return source.LeftJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs a left join with the specified selectable table.
    /// </summary>
    public static Relation LeftJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "left join");
    }


    /// <summary>
    /// Performs a right join with the specified query.
    /// </summary>
    public static Relation RightJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.RightJoin(st);
    }

    /// <summary>
    /// Performs a right join with the specified table name.
    /// </summary>
    public static Relation RightJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.RightJoin(st);
    }

    /// <summary>
    /// Performs a right join with the specified schema and table name.
    /// </summary>
    public static Relation RightJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.RightJoin(st);
    }

    /// <summary>
    /// Performs a right join with the specified common table.
    /// </summary>
    public static Relation RightJoin(this FromClause source, CommonTable table)
    {
        return source.RightJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs a right join with the specified selectable table.
    /// </summary>
    public static Relation RightJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "right join");
    }


    /// <summary>
    /// Performs a cross join with the specified query.
    /// </summary>
    public static Relation CrossJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.CrossJoin(st);
    }

    /// <summary>
    /// Performs a cross join with the specified table name.
    /// </summary>
    public static Relation CrossJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.CrossJoin(st);
    }

    /// <summary>
    /// Performs a cross join with the specified schema and table name.
    /// </summary>
    public static Relation CrossJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.CrossJoin(st);
    }

    /// <summary>
    /// Performs a cross join with the specified common table.
    /// </summary>
    public static Relation CrossJoin(this FromClause source, CommonTable table)
    {
        return source.CrossJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs a cross join with the specified selectable table.
    /// </summary>
    public static Relation CrossJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "cross join");
    }


    /// <summary>
    /// Performs a join with the specified schema, table, and join type.
    /// </summary>
    public static Relation Join(this FromClause source, string schema, string table, string join)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.Join(st, join);
    }

    /// <summary>
    /// Performs a join with the specified selectable table and join type.
    /// </summary>
    public static Relation Join(this FromClause source, SelectableTable table, string join)
    {
        var r = new Relation(table, join);

        source.Relations ??= new();
        source.Relations.Add(r);
        return r;
    }
}
