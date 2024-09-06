using Carbunql.Building;
using Carbunql.Fluent;
using Cysharp.Text;

namespace Carbunql.Fluent;

public static class SelectQueryWhereExtensions
{
    internal static char[] ParameterSymbols = { '@', ':', '$' };

    /// <summary>
    /// Conditionally applies the specified function to the query if the condition is true.
    /// </summary>
    /// <param name="query">The query to apply the function to.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="func">The function to apply if the condition is true.</param>
    /// <returns>The modified query if the condition is true; otherwise, the original query.</returns>
    public static SelectQuery If(this SelectQuery query, bool condition, Func<SelectQuery, SelectQuery> func)
    {
        return condition
            ? func(query)
            : query;
    }

    /// <summary>
    /// Applies the specified function to the query if the value is not null.
    /// </summary>
    /// <param name="query">The query to apply the function to.</param>
    /// <param name="value">The value to check for null.</param>
    /// <param name="func">The function to apply if the value is not null.</param>
    /// <returns>The modified query if the value is not null; otherwise, the original query.</returns>
    public static SelectQuery IfNotNull(this SelectQuery query, object? value, Func<SelectQuery, SelectQuery> func)
    {
        return value != null
            ? func(query)
            : query;
    }

    /// <summary>
    /// Applies the specified function to the query if the string value is not null or empty.
    /// </summary>
    /// <param name="query">The query to apply the function to.</param>
    /// <param name="value">The string value to check for null or emptiness.</param>
    /// <param name="func">The function to apply if the string value is not null or empty.</param>
    /// <returns>The modified query if the string value is not null or empty; otherwise, the original query.</returns>
    public static SelectQuery IfNotNullOrEmpty(this SelectQuery query, string? value, Func<SelectQuery, SelectQuery> func)
    {
        return !string.IsNullOrEmpty(value)
            ? func(query)
            : query;
    }

    /// <summary>
    /// Applies the specified function to the query if the provided collection contains any elements.
    /// </summary>
    /// <param name="query">The query to which the function will be applied.</param>
    /// <param name="value">The collection to check for any elements.</param>
    /// <param name="func">The function to apply to the query if the collection contains any elements.</param>
    /// <returns>The modified query if the collection contains any elements; otherwise, returns the original query.</returns>
    public static SelectQuery IfNotEmpty<T>(this SelectQuery query, IEnumerable<T> value, Func<SelectQuery, SelectQuery> func)
    {
        return value.Any()
            ? func(query)
            : query;
    }

    private static (string, string) GenerateComparison(string operatorSymbol, object? value)
    {
        if (value == null)
        {
            return operatorSymbol == "=" ? ("is", "null") : ("is not", "null");
        }
        else if (value is string s)
        {
            if (!ParameterSymbols.Where(s.StartsWith).Any())
            {
                throw new InvalidOperationException("The string type must be parameterized.");
            }
            return (operatorSymbol, s);
        }
        else if (value is DateTime)
        {
            return (operatorSymbol, $"'{value.ToString()}'");
        }
        else
        {
            return (operatorSymbol, value.ToString()!);
        }
    }

    private static void AddComparison(SelectQuery query, string columnName, string operatorSymbol, object? value)
    {
        if (value == null && operatorSymbol != "=" && operatorSymbol != "<>")
        {
            throw new InvalidOperationException("NULL has no relation to size");
        }

        var (op, val) = GenerateComparison(operatorSymbol, value);
        if (op == ">")
        {
            query.AddWhere(columnName, (source, column) => $"{val} < {column}", true);
        }
        if (op == ">=")
        {
            query.AddWhere(columnName, (source, column) => $"{val} <= {column}", true);
        }
        else
        {
            query.AddWhere(columnName, (source, column) => $"{column} {op} {val}", true);
        }
    }

    private static void AddComparison(SelectQuery query, string tableName, string columnName, string operatorSymbol, object? value)
    {
        if (value == null && operatorSymbol != "=" && operatorSymbol != "<>")
        {
            throw new InvalidOperationException("NULL has no relation to size");
        }

        var (op, val) = GenerateComparison(operatorSymbol, value);
        if (op == ">")
        {
            query.AddWhere(tableName, columnName, (source, column) => $"{val} < {column}", true);
        }
        if (op == ">=")
        {
            query.AddWhere(tableName, columnName, (source, column) => $"{val} <= {column}", true);
        }
        else
        {
            query.AddWhere(tableName, columnName, (source, column) => $"{column} {op} {val}", true);
        }
    }

    public static SelectQuery EqualIfNotNullOrEmpty(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.EqualIfNotNullOrEmpty(table.Alias, columnName, value);
    }

    public static SelectQuery EqualIfNotNullOrEmpty(this SelectQuery query, string tableName, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, tableName, columnName, "=", value);
        }
        return query;
    }

    public static SelectQuery EqualIfNotNullOrEmpty(this SelectQuery query, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, columnName, "=", value);
        }
        return query;
    }

    public static SelectQuery NotEqualIfNotNullOrEmpty(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.NotEqualIfNotNullOrEmpty(table.Alias, columnName, value);
    }

    public static SelectQuery NotEqualIfNotNullOrEmpty(this SelectQuery query, string tableName, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, tableName, columnName, "<>", value);
        }
        return query;
    }

    public static SelectQuery NotEqualIfNotNullOrEmpty(this SelectQuery query, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, columnName, "<>", value);
        }
        return query;
    }

    public static SelectQuery LessThanIfNotNullOrEmpty(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.LessThanIfNotNullOrEmpty(table.Alias, columnName, value);
    }

    public static SelectQuery LessThanIfNotNullOrEmpty(this SelectQuery query, string tableName, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, tableName, columnName, "<", value);
        }
        return query;
    }

    public static SelectQuery LessThanIfNotNullOrEmpty(this SelectQuery query, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, columnName, "<", value);
        }
        return query;
    }

    public static SelectQuery LessThanOrEqualIfNotNullOrEmpty(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.LessThanOrEqualIfNotNullOrEmpty(table.Alias, columnName, value);
    }

    public static SelectQuery LessThanOrEqualIfNotNullOrEmpty(this SelectQuery query, string tableName, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, tableName, columnName, "<=", value);
        }
        return query;
    }

    public static SelectQuery LessThanOrEqualIfNotNullOrEmpty(this SelectQuery query, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, columnName, "<=", value);
        }
        return query;
    }

    public static SelectQuery GreaterThanIfNotNullOrEmpty(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.GreaterThanIfNotNullOrEmpty(table.Alias, columnName, value);
    }

    public static SelectQuery GreaterThanIfNotNullOrEmpty(this SelectQuery query, string tableName, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, tableName, columnName, ">", value);
        }
        return query;
    }

    public static SelectQuery GreaterThanIfNotNullOrEmpty(this SelectQuery query, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, columnName, ">", value);
        }
        return query;
    }

    public static SelectQuery GreaterThanOrEqualIfNotNullOrEmpty(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.GreaterThanOrEqualIfNotNullOrEmpty(table.Alias, columnName, value);
    }

    public static SelectQuery GreaterThanOrEqualIfNotNullOrEmpty(this SelectQuery query, string tableName, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, tableName, columnName, ">=", value);
        }
        return query;
    }

    public static SelectQuery GreaterThanOrEqualIfNotNullOrEmpty(this SelectQuery query, string columnName, object? value)
    {
        if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
        {
            AddComparison(query, columnName, ">=", value);
        }
        return query;
    }

    public static SelectQuery Equal(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.Equal(table.Alias, columnName, value);
    }

    public static SelectQuery Equal(this SelectQuery query, string tableName, string columnName, object? value)
    {
        AddComparison(query, tableName, columnName, "=", value);
        return query;
    }

    public static SelectQuery Equal(this SelectQuery query, string columnName, object? value)
    {
        AddComparison(query, columnName, "=", value);
        return query;
    }

    public static SelectQuery NotEqual(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.NotEqual(table.Alias, columnName, value);
    }

    public static SelectQuery NotEqual(this SelectQuery query, string tableName, string columnName, object? value)
    {
        AddComparison(query, tableName, columnName, "<>", value);
        return query;
    }

    public static SelectQuery NotEqual(this SelectQuery query, string columnName, object? value)
    {
        AddComparison(query, columnName, "<>", value);
        return query;
    }

    public static SelectQuery LessThan(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.LessThan(table.Alias, columnName, value);
    }

    public static SelectQuery LessThan(this SelectQuery query, string tableName, string columnName, object? value)
    {
        AddComparison(query, tableName, columnName, "<", value);
        return query;
    }

    public static SelectQuery LessThan(this SelectQuery query, string columnName, object? value)
    {
        AddComparison(query, columnName, "<", value);
        return query;
    }

    public static SelectQuery LessThanOrEqual(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.LessThanOrEqual(table.Alias, columnName, value);
    }

    public static SelectQuery LessThanOrEqual(this SelectQuery query, string tableName, string columnName, object? value)
    {
        AddComparison(query, tableName, columnName, "<=", value);
        return query;
    }

    public static SelectQuery LessThanOrEqual(this SelectQuery query, string columnName, object? value)
    {
        AddComparison(query, columnName, "<=", value);
        return query;
    }

    public static SelectQuery GreaterThan(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.GreaterThan(table.Alias, columnName, value);
    }

    public static SelectQuery GreaterThan(this SelectQuery query, string tableName, string columnName, object? value)
    {
        AddComparison(query, tableName, columnName, ">", value);
        return query;
    }

    public static SelectQuery GreaterThan(this SelectQuery query, string columnName, object? value)
    {
        AddComparison(query, columnName, ">", value);
        return query;
    }

    public static SelectQuery GreaterThanOrEqual(this SelectQuery query, FluentTable table, string columnName, object? value)
    {
        return query.GreaterThanOrEqual(table.Alias, columnName, value);
    }

    public static SelectQuery GreaterThanOrEqual(this SelectQuery query, string tableName, string columnName, object? value)
    {
        AddComparison(query, tableName, columnName, ">=", value);
        return query;
    }

    public static SelectQuery GreaterThanOrEqual(this SelectQuery query, string columnName, object? value)
    {
        AddComparison(query, columnName, ">=", value);
        return query;
    }

    public static SelectQuery BetweenExclusive(this SelectQuery query, FluentTable table, string columnName, object minValue, object maxValue)
    {
        return query.BetweenExclusive(table.Alias, columnName, minValue, maxValue);
    }

    public static SelectQuery BetweenExclusive(this SelectQuery query, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, columnName, ">", minValue);
        AddComparison(query, columnName, "<", maxValue);
        return query;
    }

    public static SelectQuery BetweenExclusive(this SelectQuery query, string tableName, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, tableName, columnName, ">", minValue);
        AddComparison(query, tableName, columnName, "<", maxValue);
        return query;
    }

    public static SelectQuery BetweenInclusiveEnd(this SelectQuery query, FluentTable table, string columnName, object minValue, object maxValue)
    {
        return query.BetweenInclusiveEnd(table.Alias, columnName, minValue, maxValue);
    }

    public static SelectQuery BetweenInclusiveEnd(this SelectQuery query, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, columnName, ">", minValue);
        AddComparison(query, columnName, "<=", maxValue);
        return query;
    }

    public static SelectQuery BetweenInclusiveEnd(this SelectQuery query, string tableName, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, tableName, columnName, ">", minValue);
        AddComparison(query, tableName, columnName, "<=", maxValue);
        return query;
    }

    public static SelectQuery BetweenInclusiveStart(this SelectQuery query, FluentTable table, string columnName, object minValue, object maxValue)
    {
        return query.BetweenInclusiveStart(table.Alias, columnName, minValue, maxValue);
    }

    public static SelectQuery BetweenInclusiveStart(this SelectQuery query, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, columnName, ">=", minValue);
        AddComparison(query, columnName, "<", maxValue);
        return query;
    }

    public static SelectQuery BetweenInclusiveStart(this SelectQuery query, string tableName, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, tableName, columnName, ">=", minValue);
        AddComparison(query, tableName, columnName, "<", maxValue);
        return query;
    }

    public static SelectQuery BetweenInclusive(this SelectQuery query, FluentTable table, string columnName, object minValue, object maxValue)
    {
        return query.BetweenInclusive(table.Alias, columnName, minValue, maxValue);
    }

    public static SelectQuery BetweenInclusive(this SelectQuery query, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, columnName, ">=", minValue);
        AddComparison(query, columnName, "<=", maxValue);
        return query;
    }

    public static SelectQuery BetweenInclusive(this SelectQuery query, string tableName, string columnName, object minValue, object maxValue)
    {
        AddComparison(query, tableName, columnName, ">=", minValue);
        AddComparison(query, tableName, columnName, "<=", maxValue);
        return query;
    }

    public static SelectQuery Exists(this SelectQuery query, FluentTable table, IEnumerable<string> keyColumnNames)
    {
        if (table.IsCommonTable)
        {
            query.With(table);
        }

        query.GetQuerySources()
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsByQuery()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery().From(table);
                    keyColumnNames.ForEach(keyColumn => sq.Where($"{table.Alias}.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToExists();
                });
            });

        return query;
    }

    public static SelectQuery Exists(this SelectQuery query, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        query.AddExists(keyColumnNames, validationTableName);
        return query;
    }

    public static SelectQuery Exists(this SelectQuery query, string tableName, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        query.AddExists(tableName, keyColumnNames, validationTableName);
        return query;
    }

    public static SelectQuery NotExists(this SelectQuery query, FluentTable table, IEnumerable<string> keyColumnNames)
    {
        if (table.IsCommonTable)
        {
            query.With(table);
        }

        query.GetQuerySources()
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsByQuery()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery().From(table);
                    keyColumnNames.ForEach(keyColumn => sq.Where($"{table.Alias}.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToNotExists();
                });
            });

        return query;
    }

    public static SelectQuery NotExists(this SelectQuery query, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        query.AddNotExists(keyColumnNames, validationTableName);
        return query;
    }

    public static SelectQuery NotExists(this SelectQuery query, string tableName, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        query.AddNotExists(tableName, keyColumnNames, validationTableName);
        return query;
    }

    public static SelectQuery HasDifferent(this SelectQuery query, string leftTableAlias, string rightTableAlias, IEnumerable<string> validationColumns)
    {
        var conditions = validationColumns.Select(column =>
            $"CASE WHEN {leftTableAlias}.{column} IS NULL AND {rightTableAlias}.{column} IS NULL THEN 0 " +
            $"WHEN {leftTableAlias}.{column} IS NOT NULL AND {rightTableAlias}.{column} IS NULL THEN 1 " +
            $"WHEN {leftTableAlias}.{column} IS NULL AND {rightTableAlias}.{column} IS NOT NULL THEN 1 " +
            $"WHEN {leftTableAlias}.{column} <> {rightTableAlias}.{column} THEN 1 " +
            "ELSE 0 END"
        );

        var fullCondition = string.Join(" + ", conditions);
        query.AddWhere($"{fullCondition} > 0");

        return query;
    }

    public static SelectQuery HasDifferent(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> validationColumns)
    {
        return query.HasDifferent(left.Alias, right.Alias, validationColumns);
    }

    /// <summary>
    /// Adds an `IN` condition to the query where the specified column value matches any value in the given collection.
    /// </summary>
    /// <typeparam name="T">The type of the values in the collection. Must be a non-nullable type.</typeparam>
    /// <param name="query">The query to which the `IN` condition will be added.</param>
    /// <param name="columnName">The name of the column to check against the values.</param>
    /// <param name="values">The collection of values to match against. Values are converted to strings and added to the condition.</param>
    /// <returns>The modified query with the `IN` condition applied.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the type of any string value is not parameterized properly.</exception>
    public static SelectQuery In<T>(this SelectQuery query, string columnName, IEnumerable<T> values) where T : notnull
    {
        var sb = ZString.CreateStringBuilder();
        foreach (var item in values)
        {
            // Check if the type is string
            if (typeof(T) == typeof(string))
            {
                var stringItem = item as string;
                if (stringItem != null && !ParameterSymbols.Any(stringItem.StartsWith))
                {
                    throw new InvalidOperationException("The string type must be parameterized.");
                }
            }

            if (sb.Length != 0) sb.Append(", ");

            if (typeof(T) == typeof(DateTime) && item is DateTime dateItem)
            {
                sb.Append($"'{dateItem.ToString()}'");
            }
            else
            {
                sb.Append(item);
            }
        }

        query.AddWhere(columnName, (source, column) => $"{column} in ({sb.ToString()})", true);

        return query;
    }

    /// <summary>
    /// Adds an `IN` condition to the query where the specified column value matches any value in the results of the provided subquery.
    /// </summary>
    /// <param name="query">The query to which the `IN` condition will be added.</param>
    /// <param name="columnName">The name of the column to check against the subquery results.</param>
    /// <param name="selectQuery">The subquery whose results will be used for the `IN` condition.</param>
    /// <returns>The modified query with the `IN` condition applied.</returns>
    public static SelectQuery In(this SelectQuery query, string columnName, SelectQuery selectQuery)
    {
        // Import parameters from the subquery
        selectQuery.GetParameters().ForEach(p =>
        {
            query.AddParameter(p.ParameterName, p.Value);
        });

        query.AddWhere(columnName, (source, column) => $"{column} in ({selectQuery.ToOneLineText()})", true);

        return query;
    }

    /// <summary>
    /// Adds an `IN` condition to the query where the specified column value matches any value in the results of the provided subquery.
    /// This version allows specifying the table name for more complex queries.
    /// </summary>
    /// <param name="query">The query to which the `IN` condition will be added.</param>
    /// <param name="tableName">The name of the table to apply the condition to.</param>
    /// <param name="columnName">The name of the column to check against the subquery results.</param>
    /// <param name="selectQuery">The subquery whose results will be used for the `IN` condition.</param>
    /// <returns>The modified query with the `IN` condition applied.</returns>
    public static SelectQuery In(this SelectQuery query, string tableName, string columnName, SelectQuery selectQuery)
    {
        // Import parameters from the subquery
        selectQuery.GetParameters().ForEach(p =>
        {
            query.AddParameter(p.ParameterName, p.Value);
        });

        query.AddWhere(tableName, columnName, (source, column) => $"{column} in ({selectQuery.ToOneLineText()})");

        return query;
    }

    /// <summary>
    /// Applies an `ANY` condition to the query, checking if the specified column value matches any value in the array parameter.
    /// </summary>
    /// <param name="query">The query to which the condition will be applied.</param>
    /// <param name="columnName">The name of the column to check against the array values.</param>
    /// <param name="arrayParameter">The parameter representing the array of values to match against. This should be a parameterized placeholder.</param>
    /// <returns>The modified query with the `ANY` condition applied.</returns>
    public static SelectQuery Any(this SelectQuery query, string columnName, string arrayParameter)
    {
        if (!ParameterSymbols.Any(arrayParameter.StartsWith))
        {
            throw new InvalidOperationException("The string type must be parameterized.");
        }

        query.AddWhere(columnName, (source, column) => $"{column} = any ({arrayParameter})", true);

        return query;
    }
}
