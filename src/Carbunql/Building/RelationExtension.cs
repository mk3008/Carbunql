using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class RelationExtension
{
    /// <summary>
    /// Sets an alias for the table in the relation.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The modified relation.</returns>
    public static Relation As(this Relation source, string alias)
    {
        source.Table.SetAlias(alias);
        return source;
    }

    /// <summary>
    /// Specifies the ON condition for the relation based on a single column.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="from">The source table in the FROM clause.</param>
    /// <param name="column">The column name for the ON condition.</param>
    /// <returns>The selectable table.</returns>
    public static SelectableTable On(this Relation source, FromClause from, string column)
    {
        return source.On(from.Root, new[] { column });
    }

    /// <summary>
    /// Specifies the ON condition for the relation based on a single column.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="sourceTable">The source table.</param>
    /// <param name="column">The column name for the ON condition.</param>
    /// <returns>The selectable table.</returns>
    public static SelectableTable On(this Relation source, SelectableTable sourceTable, string column)
    {
        return source.On(sourceTable, new[] { column });
    }

    /// <summary>
    /// Specifies the ON condition for the relation based on multiple columns.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="sourceTable">The source table.</param>
    /// <param name="columns">The column names for the ON condition.</param>
    /// <returns>The selectable table.</returns>
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

    /// <summary>
    /// Specifies the ON condition for the relation using a custom builder function.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="builder">The builder function to define the ON condition.</param>
    /// <returns>The selectable table.</returns>
    public static SelectableTable On(this Relation source, Func<Relation, ValueBase> builder)
    {
        source.Condition = builder(source);
        return source.Table;
    }

    /// <summary>
    /// Specifies the ON condition for the relation using a custom builder action.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="builder">The builder action to define the ON condition.</param>
    /// <returns>The selectable table.</returns>
    public static SelectableTable On(this Relation source, Action<Relation> builder)
    {
        builder(source);
        return source.Table;
    }

    /// <summary>
    /// Specifies a condition for the relation based on a column.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="table">The table name.</param>
    /// <param name="column">The column name.</param>
    /// <returns>The value base representing the condition.</returns>
    public static ValueBase Condition(this Relation source, string table, string column)
    {
        var v = new ColumnValue(table, column);
        return source.Condition(v);
    }

    /// <summary>
    /// Specifies a condition for the relation based on a column in the FROM clause.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="table">The FROM clause table.</param>
    /// <param name="column">The column name.</param>
    /// <returns>The value base representing the condition.</returns>
    public static ValueBase Condition(this Relation source, FromClause table, string column)
    {
        var v = new ColumnValue(table, column);
        return source.Condition(v);
    }

    /// <summary>
    /// Specifies a condition for the relation based on a column in a selectable table.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="table">The selectable table.</param>
    /// <param name="column">The column name.</param>
    /// <returns>The value base representing the condition.</returns>
    public static ValueBase Condition(this Relation source, SelectableTable table, string column)
    {
        var v = new ColumnValue(table, column);
        return source.Condition(v);
    }

    /// <summary>
    /// Specifies a WHERE condition for the relation.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="text">The text of the WHERE condition.</param>
    /// <returns>The value base representing the condition.</returns>
    public static ValueBase Where(this Relation source, string text)
    {
        var v = ValueParser.Parse(text);
        return source.Condition(v);
    }

    /// <summary>
    /// Specifies a condition for the relation using a custom builder function.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="builder">The builder function to define the condition.</param>
    /// <returns>The value base representing the condition.</returns>
    public static ValueBase Condition(this Relation source, Func<ValueBase> builder)
    {
        var v = builder();
        return source.Condition(v);
    }

    /// <summary>
    /// Specifies a condition for the relation.
    /// </summary>
    /// <param name="source">The source relation.</param>
    /// <param name="value">The value base representing the condition.</param>
    /// <returns>The value base representing the condition.</returns>
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
