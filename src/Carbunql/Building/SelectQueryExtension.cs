using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;

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
        var subQuery = new SelectQuery(query);
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
}