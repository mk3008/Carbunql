using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for building WHERE clauses in SELECT queries.
/// </summary>
public static class WhereClauseExtension
{
    /// <summary>
    /// Adds a WHERE clause to the SELECT query using the specified table and column.
    /// </summary>
    /// <param name="source">The SELECT query.</param>
    /// <param name="table">The table name.</param>
    /// <param name="column">The column name.</param>
    /// <returns>The generated WHERE clause.</returns>
    public static ValueBase Where(this SelectQuery source, string table, string column)
    {
        var v = new ColumnValue(table, column);
        return source.Where(v);
    }

    /// <summary>
    /// Adds a WHERE clause to the SELECT query using the specified table and column from a FROM clause.
    /// </summary>
    /// <param name="source">The SELECT query.</param>
    /// <param name="table">The FROM clause representing the table.</param>
    /// <param name="column">The column name.</param>
    /// <returns>The generated WHERE clause.</returns>
    public static ValueBase Where(this SelectQuery source, FromClause table, string column)
    {
        var v = new ColumnValue(table, column);
        return source.Where(v);
    }

    /// <summary>
    /// Adds a WHERE clause to the SELECT query using the specified table and column from a selectable table.
    /// </summary>
    /// <param name="source">The SELECT query.</param>
    /// <param name="table">The selectable table.</param>
    /// <param name="column">The column name.</param>
    /// <returns>The generated WHERE clause.</returns>
    public static ValueBase Where(this SelectQuery source, SelectableTable table, string column)
    {
        var v = new ColumnValue(table, column);
        return source.Where(v);
    }

    /// <summary>
    /// Adds a WHERE clause to the SELECT query using the specified text.
    /// </summary>
    /// <param name="source">The SELECT query.</param>
    /// <param name="text">The text representing the condition.</param>
    /// <returns>The generated WHERE clause.</returns>
    public static ValueBase Where(this SelectQuery source, string text)
    {
        var v = ValueParser.Parse(text);
        return source.Where(v);
    }

    /// <summary>
    /// Adds a WHERE clause to the SELECT query using a custom builder function.
    /// </summary>
    /// <param name="source">The SELECT query.</param>
    /// <param name="builder">The function to build the condition.</param>
    /// <returns>The generated WHERE clause.</returns>
    public static ValueBase Where(this SelectQuery source, Func<ValueBase> builder)
    {
        var v = builder();
        return source.Where(v);
    }

    /// <summary>
    /// Adds a WHERE clause to the SELECT query using the specified value base.
    /// </summary>
    /// <param name="source">The SELECT query.</param>
    /// <param name="value">The value base representing the condition.</param>
    /// <returns>The generated WHERE clause.</returns>
    public static ValueBase Where(this SelectQuery source, ValueBase value)
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
        return value;
    }
}
