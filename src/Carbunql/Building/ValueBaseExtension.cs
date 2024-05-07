using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class ValueBaseExtension
{
    /// <summary>
    /// Retrieves the last value in a chain of operatable values.
    /// </summary>
    /// <param name="source">The initial value.</param>
    /// <returns>The last value in the chain.</returns>
    public static ValueBase GetLast(this ValueBase source)
    {
        if (source.OperatableValue == null) return source;
        return source.OperatableValue.Value.GetLast();
    }

    /// <summary>
    /// Creates an equality condition between the source value and a text.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="text">The text value to compare with.</param>
    /// <returns>The modified source value with the equality condition.</returns>
    public static ValueBase Equal(this ValueBase source, string text)
    {
        return source.Equal(ValueParser.Parse(text));
    }

    /// <summary>
    /// Creates an equality condition between the source value and an integer value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="value">The integer value to compare with.</param>
    /// <returns>The modified source value with the equality condition.</returns>
    public static ValueBase Equal(this ValueBase source, int value)
    {
        return source.Equal(new LiteralValue(value.ToString()));
    }

    /// <summary>
    /// Creates an equality condition between the source value and a column in a table.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The name of the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the equality condition.</returns>
    public static ValueBase Equal(this ValueBase source, string table, string column)
    {
        return source.Equal(new ColumnValue(table, column));
    }

    /// <summary>
    /// Creates an equality condition between the source value and a column in a table specified in a from clause.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The from clause representing the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the equality condition.</returns>
    public static ValueBase Equal(this ValueBase source, FromClause table, string column)
    {
        return source.Equal(new ColumnValue(table, column));
    }

    /// <summary>
    /// Creates an equality condition between the source value and a column in a selectable table.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The selectable table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the equality condition.</returns>
    public static ValueBase Equal(this ValueBase source, SelectableTable table, string column)
    {
        return source.Equal(new ColumnValue(table, column));
    }

    /// <summary>
    /// Creates an equality condition between the source value and another value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="operand">The value to compare with.</param>
    /// <returns>The modified source value with the equality condition.</returns>
    public static ValueBase Equal(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("=", operand);
        return source;
    }

    /// <summary>
    /// Creates a not equal condition between the source value and a text.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="text">The text value to compare with.</param>
    /// <returns>The modified source value with the not equal condition.</returns>
    public static ValueBase NotEqual(this ValueBase source, string text)
    {
        return source.NotEqual(ValueParser.Parse(text));
    }

    /// <summary>
    /// Creates a not equal condition between the source value and an integer value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="value">The integer value to compare with.</param>
    /// <returns>The modified source value with the not equal condition.</returns>
    public static ValueBase NotEqual(this ValueBase source, int value)
    {
        return source.NotEqual(new LiteralValue(value.ToString()));
    }

    /// <summary>
    /// Creates a not equal condition between the source value and a column in a table.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The name of the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the not equal condition.</returns>
    public static ValueBase NotEqual(this ValueBase source, string table, string column)
    {
        return source.NotEqual(new ColumnValue(table, column));
    }

    /// <summary>
    /// Creates a not equal condition between the source value and a column in a table specified in a from clause.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The from clause representing the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the not equal condition.</returns>
    public static ValueBase NotEqual(this ValueBase source, FromClause table, string column)
    {
        return source.NotEqual(new ColumnValue(table, column));
    }

    /// <summary>
    /// Creates a not equal condition between the source value and a column in a selectable table.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The selectable table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the not equal condition.</returns>
    public static ValueBase NotEqual(this ValueBase source, SelectableTable table, string column)
    {
        return source.NotEqual(new ColumnValue(table, column));
    }

    /// <summary>
    /// Creates a not equal condition between the source value and another value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="operand">The value to compare with.</param>
    /// <returns>The modified source value with the not equal condition.</returns>
    public static ValueBase NotEqual(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("<>", operand);
        return source;
    }

    /// <summary>
    /// Creates an is null condition for the source value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The modified source value with the is null condition.</returns>
    public static ValueBase IsNull(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("is", new LiteralValue("null"));
        return source;
    }

    /// <summary>
    /// Creates an is not null condition for the source value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The modified source value with the is not null condition.</returns>
    public static ValueBase IsNotNull(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("is not", new LiteralValue("null"));
        return source;
    }

    /// <summary>
    /// Creates a true condition for the source value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The modified source value with the true condition.</returns>
    public static ValueBase True(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("=", new LiteralValue("true"));
        return source;
    }

    /// <summary>
    /// Creates a false condition for the source value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The modified source value with the false condition.</returns>
    public static ValueBase False(this ValueBase source)
    {
        source.GetLast().AddOperatableValue("=", new LiteralValue("false"));
        return source;
    }

    /// <summary>
    /// Creates an expression using the specified operator and operand.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="operator">The operator to use in the expression.</param>
    /// <param name="operand">The operand for the expression.</param>
    /// <returns>The modified source value with the expression.</returns>
    public static ValueBase Expression(this ValueBase source, string @operator, ValueBase operand)
    {
        source.GetLast().AddOperatableValue(@operator, operand);
        return source;
    }

    /// <summary>
    /// Creates an expression using the specified operator and a value generated by the provided builder function.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="operator">The operator to use in the expression.</param>
    /// <param name="builder">The function that builds the value for the expression.</param>
    /// <returns>The modified source value with the expression.</returns>
    public static ValueBase Expression(this ValueBase source, string @operator, Func<ValueBase> builder)
    {
        source.GetLast().AddOperatableValue(@operator, builder());
        return source;
    }

    /// <summary>
    /// Combines the source value with another value using the AND logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="text">The text value to combine with.</param>
    /// <returns>The modified source value with the AND condition.</returns>
    public static ValueBase And(this ValueBase source, string text)
    {
        return source.And(ValueParser.Parse(text));
    }

    /// <summary>
    /// Combines the source value with a column in a table using the AND logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The name of the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the AND condition.</returns>
    public static ValueBase And(this ValueBase source, string table, string column)
    {
        return source.And(new ColumnValue(table, column));
    }

    /// <summary>
    /// Combines the source value with a column in a table specified in a from clause using the AND logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The from clause representing the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the AND condition.</returns>
    public static ValueBase And(this ValueBase source, FromClause table, string column)
    {
        return source.And(new ColumnValue(table, column));
    }

    /// <summary>
    /// Combines the source value with a column in a selectable table using the AND logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The selectable table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the AND condition.</returns>
    public static ValueBase And(this ValueBase source, SelectableTable table, string column)
    {
        return source.And(new ColumnValue(table, column));
    }

    /// <summary>
    /// Combines the source value with a value generated by the provided builder function using the AND logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="builder">The function that builds the value for the condition.</param>
    /// <returns>The modified source value with the AND condition.</returns>
    public static ValueBase And(this ValueBase source, Func<ValueBase> builder)
    {
        return source.And(builder());
    }

    /// <summary>
    /// Combines the source value with another value using the AND logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="operand">The value to combine with.</param>
    /// <returns>The modified source value with the AND condition.</returns>
    public static ValueBase And(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("and", operand);
        return source;
    }

    /// <summary>
    /// Combines the source value with another value using the OR logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="text">The text value to combine with.</param>
    /// <returns>The modified source value with the OR condition.</returns>
    public static ValueBase Or(this ValueBase source, string text)
    {
        return source.Or(ValueParser.Parse(text));
    }

    /// <summary>
    /// Combines the source value with a column in a table using the OR logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The name of the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the OR condition.</returns>
    public static ValueBase Or(this ValueBase source, string table, string column)
    {
        return source.Or(new ColumnValue(table, column));
    }

    /// <summary>
    /// Combines the source value with a column in a table specified in a from clause using the OR logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The from clause representing the table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the OR condition.</returns>
    public static ValueBase Or(this ValueBase source, FromClause table, string column)
    {
        return source.Or(new ColumnValue(table, column));
    }

    /// <summary>
    /// Combines the source value with a column in a selectable table using the OR logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="table">The selectable table.</param>
    /// <param name="column">The name of the column in the table.</param>
    /// <returns>The modified source value with the OR condition.</returns>
    public static ValueBase Or(this ValueBase source, SelectableTable table, string column)
    {
        return source.Or(new ColumnValue(table, column));
    }

    /// <summary>
    /// Combines the source value with another value generated by the provided builder function using the OR logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="builder">The function that builds the value for the condition.</param>
    /// <returns>The modified source value with the OR condition.</returns>
    public static ValueBase Or(this ValueBase source, Func<ValueBase> builder)
    {
        return source.Or(builder());
    }

    /// <summary>
    /// Combines the source value with another value using the OR logical operator.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="operand">The value to combine with.</param>
    /// <returns>The modified source value with the OR condition.</returns>
    public static ValueBase Or(this ValueBase source, ValueBase operand)
    {
        source.GetLast().AddOperatableValue("or", operand);
        return source;
    }

    /// <summary>
    /// Converts the source value into a group using parentheses.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The source value enclosed in parentheses.</returns>
    public static ValueBase ToGroup(this ValueBase source)
    {
        return new BracketValue(source);
    }

    /// <summary>
    /// Converts the source value into a sortable item for use in order by clauses.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="isAscending">Specifies whether the sorting is ascending (true) or descending (false).</param>
    /// <returns>The source value as a sortable item.</returns>
    public static SortableItem ToSortable(this ValueBase source, bool isAscending = true)
    {
        return new SortableItem(source) { IsAscending = isAscending };
    }

    /// <summary>
    /// Merges multiple values into a single value using the specified operator.
    /// </summary>
    /// <param name="source">The source values to merge.</param>
    /// <param name="operator">The operator used to merge the values (e.g., "and" or "or").</param>
    /// <returns>The merged value.</returns>
    public static ValueBase Merge(this IEnumerable<ValueBase> source, string @operator)
    {
        ValueBase? v = null;

        foreach (var item in source)
        {
            if (v == null)
            {
                v = item;
                continue;
            }
            v.GetLast().AddOperatableValue(@operator, item);
        }

        if (v == null) throw new NullReferenceException();
        return v;
    }

    /// <summary>
    /// Merges multiple values into a single value using the "and" operator.
    /// </summary>
    /// <param name="source">The source values to merge.</param>
    /// <returns>The merged value with "and" operator.</returns>
    public static ValueBase MergeAnd(this IEnumerable<ValueBase> source)
    {
        return Merge(source, "and");
    }

    /// <summary>
    /// Merges multiple values into a single value using the "or" operator.
    /// </summary>
    /// <param name="source">The source values to merge.</param>
    /// <returns>The merged value with "or" operator.</returns>
    public static ValueBase MergeOr(this IEnumerable<ValueBase> source)
    {
        return Merge(source, "or");
    }
}
