using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for building SQL FROM clauses.
/// </summary>
public static class FromClauseExtension
{
    /// <summary>
    /// Sets an alias for the root table in the FROM clause.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="alias">The alias to be set for the root table.</param>
    /// <returns>A tuple containing the updated FROM clause and the root table with the alias set.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the alias parameter is null or empty.</exception>
    public static (FromClause, SelectableTable) As(this FromClause source, string alias)
    {
        if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));
        source.Root.SetAlias(alias);
        return (source, source.Root);
    }

    /// <summary>
    /// Performs an inner join with the specified query, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="query">The query to join with.</param>
    /// <returns>A Relation representing the inner join with the specified query.</returns>
    public static Relation InnerJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.InnerJoin(st);
    }

    /// <summary>
    /// Performs an inner join with the specified table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the inner join with the specified table.</returns>
    public static Relation InnerJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.InnerJoin(st);
    }

    /// <summary>
    /// Performs an inner join with the specified schema and table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="schema">The name of the schema of the table to join with.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the inner join with the specified table.</returns>
    public static Relation InnerJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.InnerJoin(st);
    }

    /// <summary>
    /// Performs an inner join with the specified common table, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The common table to join with.</param>
    /// <returns>A Relation representing the inner join with the specified common table.</returns>
    public static Relation InnerJoin(this FromClause source, CommonTable table)
    {
        return source.InnerJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs an inner join with the specified selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The selectable table to join with.</param>
    /// <returns>A Relation representing the inner join with the specified table.</returns>
    public static Relation InnerJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "inner join");
    }

    /// <summary>
    /// Performs a left join with the specified query, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="query">The query to join with.</param>
    /// <returns>A Relation representing the left join with the specified query.</returns>
    public static Relation LeftJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.LeftJoin(st);
    }

    /// <summary>
    /// Performs a left join with the specified table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the left join with the specified table.</returns>
    public static Relation LeftJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.LeftJoin(st);
    }

    /// <summary>
    /// Performs a left join with the specified schema and table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="schema">The name of the schema of the table to join with.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the left join with the specified table.</returns>
    public static Relation LeftJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.LeftJoin(st);
    }

    /// <summary>
    /// Performs a left join with the specified common table, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The common table to join with.</param>
    /// <returns>A Relation representing the left join with the specified common table.</returns>
    public static Relation LeftJoin(this FromClause source, CommonTable table)
    {
        return source.LeftJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs a left join with the specified selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The selectable table to join with.</param>
    /// <returns>A Relation representing the left join with the specified table.</returns>
    public static Relation LeftJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "left join");
    }

    /// <summary>
    /// Performs a right join with the specified query, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="query">The query to join with.</param>
    /// <returns>A Relation representing the right join with the specified query.</returns>
    public static Relation RightJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.RightJoin(st);
    }

    /// <summary>
    /// Performs a right join with the specified table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the right join with the specified table.</returns>
    public static Relation RightJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.RightJoin(st);
    }

    /// <summary>
    /// Performs a right join with the specified schema and table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="schema">The name of the schema of the table to join with.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the right join with the specified table.</returns>
    public static Relation RightJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.RightJoin(st);
    }

    /// <summary>
    /// Performs a right join with the specified common table, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The common table to join with.</param>
    /// <returns>A Relation representing the right join with the specified common table.</returns>
    public static Relation RightJoin(this FromClause source, CommonTable table)
    {
        return source.RightJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs a right join with the specified selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The selectable table to join with.</param>
    /// <returns>A Relation representing the right join with the specified table.</returns>
    public static Relation RightJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "right join");
    }

    /// <summary>
    /// Performs a cross join with the specified query, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="query">The query to join with.</param>
    /// <returns>A Relation representing the cross join with the specified query.</returns>
    public static Relation CrossJoin(this FromClause source, IReadQuery query)
    {
        var st = query.ToSelectableTable();
        return source.CrossJoin(st);
    }

    /// <summary>
    /// Performs a cross join with the specified table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the cross join with the specified table.</returns>
    public static Relation CrossJoin(this FromClause source, string table)
    {
        var st = new PhysicalTable(table).ToSelectable();
        return source.CrossJoin(st);
    }

    /// <summary>
    /// Performs a cross join with the specified schema and table name, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="schema">The name of the schema of the table to join with.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <returns>A Relation representing the cross join with the specified table.</returns>
    public static Relation CrossJoin(this FromClause source, string schema, string table)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.CrossJoin(st);
    }

    /// <summary>
    /// Performs a cross join with the specified common table, converting it to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The common table to join with.</param>
    /// <returns>A Relation representing the cross join with the specified common table.</returns>
    public static Relation CrossJoin(this FromClause source, CommonTable table)
    {
        return source.CrossJoin(table.ToPhysicalTable().ToSelectable());
    }

    /// <summary>
    /// Performs a cross join with the specified selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The selectable table to join with.</param>
    /// <returns>A Relation representing the cross join with the specified table.</returns>
    public static Relation CrossJoin(this FromClause source, SelectableTable table)
    {
        return source.Join(table, "cross join");
    }

    /// <summary>
    /// Performs a join with the specified schema, table, and join type, converting the table to a selectable table.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="schema">The name of the schema of the table to join with.</param>
    /// <param name="table">The name of the table to join with.</param>
    /// <param name="join">The type of join operation.</param>
    /// <returns>A Relation representing the join with the specified table.</returns>
    public static Relation Join(this FromClause source, string schema, string table, string join)
    {
        var st = new PhysicalTable(schema, table).ToSelectable();
        return source.Join(st, join);
    }

    /// <summary>
    /// Performs a join with the specified selectable table and join type.
    /// </summary>
    /// <param name="source">The source FROM clause.</param>
    /// <param name="table">The selectable table to join with.</param>
    /// <param name="join">The type of join operation.</param>
    /// <returns>A Relation representing the join with the specified table.</returns>
    public static Relation Join(this FromClause source, SelectableTable table, string join)
    {
        var r = new Relation(table, join);

        source.Relations ??= new List<Relation>();
        source.Relations.Add(r);
        return r;
    }
}
