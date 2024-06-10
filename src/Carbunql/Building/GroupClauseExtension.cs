using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for adding grouping clauses to a SelectQuery.
/// </summary>
public static class GroupClauseExtension
{
    /// <summary>
    /// Adds a grouping clause to the select query using the specified table and column.
    /// </summary>
    /// <param name="source">The select query.</param>
    /// <param name="from">The source table from which to select.</param>
    /// <param name="column">The column to group by.</param>
    public static void Group(this SelectQuery source, FromClause from, string column)
    {
        source.Group(from.Root.Alias, column);
    }

    /// <summary>
    /// Adds a grouping clause to the select query using the specified table and column.
    /// </summary>
    /// <param name="source">The select query.</param>
    /// <param name="table">The selectable table from which to select.</param>
    /// <param name="column">The column to group by.</param>
    public static void Group(this SelectQuery source, SelectableTable table, string column)
    {
        source.Group(table.Alias, column);
    }

    /// <summary>
    /// Adds a grouping clause to the select query using the specified selectable item.
    /// </summary>
    /// <param name="source">The select query.</param>
    /// <param name="item">The selectable item to group by.</param>
    public static void Group(this SelectQuery source, SelectableItem item)
    {
        source.GroupClause ??= new();
        source.GroupClause.Add(item.Value);
    }

    /// <summary>
    /// Adds a grouping clause to the select query using the specified table and column.
    /// </summary>
    /// <param name="source">The select query.</param>
    /// <param name="table">The table name or alias.</param>
    /// <param name="column">The column to group by.</param>
    public static void Group(this SelectQuery source, string table, string column)
    {
        var item = new ColumnValue(table, column);
        source.GroupClause ??= new();
        source.GroupClause.Add(item);
    }

    /// <summary>
    /// Adds a grouping clause to the select query using the specified selectable item.
    /// </summary>
    /// <param name="source">The select query.</param>
    /// <param name="value">The valuebase item to group by.</param>
    public static void Group(this SelectQuery source, ValueBase item)
    {
        source.GroupClause ??= new();
        source.GroupClause.Add(item);
    }
}
