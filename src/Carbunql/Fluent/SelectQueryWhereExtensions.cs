using Carbunql.Extensions;
using Carbunql.Building;
using Carbunql.Fluent;
using Cysharp.Text;

namespace Carbunql.Fluent;

public static class SelectQueryWhereExtensions
{
    internal static char[] ParameterSymbols = { '@', ':', '$' };

    /// <summary>
    /// Conditionally applies the specified function to the query if the given condition is true.
    /// </summary>
    /// <param name="query">The query to which the function will be applied.</param>
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
    /// Conditionally applies the specified function to the query if the validation function returns true.
    /// </summary>
    /// <param name="query">The query to which the function will be applied.</param>
    /// <param name="validation">The validation function to evaluate against the query.</param>
    /// <param name="func">The function to apply if the validation function returns true.</param>
    /// <returns>The modified query if the validation function returns true; otherwise, the original query.</returns>
    public static SelectQuery If(this SelectQuery query, Func<SelectQuery, bool> validation, Func<SelectQuery, SelectQuery> func)
    {
        return validation(query)
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

    /// <summary>
    /// Checks whether the specified column name exists in the query.
    /// You can specify whether to include aliases in the check.
    /// </summary>
    /// <param name="query">The <see cref="SelectQuery"/> object to search.</param>
    /// <param name="columnName">The column name to check for.</param>
    /// <param name="isAliasIncluded">If true, aliases are included in the check. The default value is true.</param>
    /// <returns>Returns true if the specified column name exists in the query; otherwise, false.</returns>
    public static bool HasColumn(this SelectQuery query, string columnName, bool isAliasIncluded = true)
    {
        return query.GetQuerySources()
                 .Where(x => x.HasColumn(columnName, isAliasIncluded)).Any();
    }

    internal static bool HasColumn(this IQuerySource source, string columnName, bool isAliasIncluded)
    {
        if (isAliasIncluded && source.Query.GetColumnNames().Where(x => x.IsEqualNoCase(columnName)).Any())
        {
            return true;
        }
        else
        {
            return source.ColumnNames.Where(x => x.IsEqualNoCase(columnName)).Any();
        }
    }

    internal static string GetColumn(this IQuerySource x, string columnName, bool isAliasIncluded)
    {
        if (x.Query.SelectClause != null && isAliasIncluded)
        {
            var selectableItem = x.Query.SelectClause!.Where(x => x.Alias.IsEqualNoCase(columnName)).FirstOrDefault();
            if (selectableItem != null)
            {
                return selectableItem.Value.ToOneLineText();
            }
        }

        var column = x.ColumnNames.Where(x => x.IsEqualNoCase(columnName)).FirstOrDefault();
        if (column != null)
        {
            return $"{x.Alias}.{column}";
        }

        throw new InvalidProgramException();
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

    [Obsolete("use 'Exists(this SelectQuery query, IEnumerable<string> keyColumnNames, FluentTable validationTable)'")]
    public static SelectQuery Exists(this SelectQuery query, FluentTable validationTable, IEnumerable<string> keyColumnNames)
    {
        return query.Exists(keyColumnNames, validationTable);
    }

    /// <summary>
    /// Applies an Exists query to the specified validation table based on the key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    [Obsolete("use ExistsIn")]
    public static SelectQuery Exists(this SelectQuery query, IEnumerable<string> keyColumnNames, FluentTable validationTable)
    {
        return query.ExistsIn(validationTable, keyColumnNames);
    }

    /// <summary>
    /// Applies an Exists query to the specified validation table based on the given table and key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="table">The FluentTable object to operate on.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    [Obsolete("use ExistsIn")]
    public static SelectQuery Exists(this SelectQuery query, FluentTable table, IEnumerable<string> keyColumnNames, FluentTable validationTable)
    {
        return query.ExistsIn(validationTable, keyColumnNames, table);
    }

    [Obsolete("use ExistsIn")]
    public static SelectQuery Exists(this SelectQuery query, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        return query.ExistsIn(validationTableName, keyColumnNames);
    }

    [Obsolete("use ExistsIn")]
    public static SelectQuery Exists(this SelectQuery query, string tableName, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        return query.ExistsIn(validationTableName, keyColumnNames, tableName);
    }

    /// <summary>
    /// Applies an Exists query to the specified validation table based on the key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    public static SelectQuery ExistsIn(this SelectQuery query, FluentTable validationTable, IEnumerable<string> keyColumnNames)
    {
        if (validationTable.IsCommonTable)
        {
            query.With(validationTable);
        }

        query.GetQuerySources()
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsByQuery()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery().From(validationTable);
                    keyColumnNames.ForEach(keyColumn => sq.Where($"{validationTable.Alias}.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToExists();
                });
            });

        return query;
    }

    public static SelectQuery ExistsIn(this SelectQuery query, FluentTable validationTable, string condition)
    {
        if (validationTable.IsCommonTable)
        {
            query.With(validationTable);
        }

        query.Where(() =>
        {
            var sq = new SelectQuery().From(validationTable);
            sq.Where(condition);
            return sq.ToExists();
        });

        return query;
    }

    public static SelectQuery ExistsIn(this SelectQuery query, FluentTable validationTable, IEnumerable<string> validationcolumns, FluentTable table, IEnumerable<string> columns)
    {
        if (columns.Count() != validationcolumns.Count())
        {
            throw new ArgumentException("The number of elements in columns and validationcolumns must be the same.");
        }

        var columnMaps = columns.Zip(validationcolumns, (left, right) => (left, right));

        var lquery = new SelectQuery().From(table).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(validationTable).GetCurrentQuerySource();

        var condition = string.Join(" and ", columnMaps.Select(item => $"{rquery.GetColumn(item.right, isAliasIncluded: true)} = {lquery.GetColumn(item.left, isAliasIncluded: true)}"));

        var targetQuery = query.GetQuerySources()
                    .Where(x => x.HasTable(table.Alias, true))
                    .EnsureAny($"table:{table.Alias}")
                    .GetRootsByQuery()
                    .First()
                    .Query;

        targetQuery.ExistsIn(validationTable, condition);

        return query;
    }

    /// <summary>
    /// Applies an Exists query to the specified validation table based on the given table and key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <param name="table">The FluentTable object to operate on.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    public static SelectQuery ExistsIn(this SelectQuery query, FluentTable validationTable, IEnumerable<string> keyColumnNames, FluentTable table)
    {
        if (validationTable.IsCommonTable)
        {
            query.With(validationTable);
        }

        query.GetQuerySources()
            .Where(x => x.HasTable(table.Alias, true))
            .EnsureAny($"table:{table.Alias}")
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsByQuery()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery().From(validationTable);
                    keyColumnNames.ForEach(keyColumn => sq.Where($"{validationTable.Alias}.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToExists();
                });
            });

        return query;
    }

    public static SelectQuery ExistsIn(this SelectQuery query, string validationTableName, IEnumerable<string> keyColumnNames)
    {
        query.AddExists(keyColumnNames, validationTableName);
        return query;
    }

    public static SelectQuery ExistsIn(this SelectQuery query, string validationTableName, IEnumerable<string> keyColumnNames, string tableName)
    {
        query.AddExists(tableName, keyColumnNames, validationTableName);
        return query;
    }

    [Obsolete("use 'NotExists(this SelectQuery query, IEnumerable<string> keyColumnNames, FluentTable validationTable)'")]
    public static SelectQuery NotExists(this SelectQuery query, FluentTable validationTable, IEnumerable<string> keyColumnNames)
    {
        return query.NotExists(keyColumnNames, validationTable);
    }

    /// <summary>
    /// Applies a NotExists query to the specified validation table based on the key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    [Obsolete("use NotExistsIn")]
    public static SelectQuery NotExists(this SelectQuery query, IEnumerable<string> keyColumnNames, FluentTable validationTable)
    {
        return query.NotExistsIn(validationTable, keyColumnNames);
    }

    /// <summary>
    /// Applies a NotExists query to the specified validation table based on the given table and key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="table">The FluentTable object to operate on.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    [Obsolete("use NotExistsIn")]
    public static SelectQuery NotExists(this SelectQuery query, FluentTable table, IEnumerable<string> keyColumnNames, FluentTable validationTable)
    {
        return query.NotExistsIn(validationTable, keyColumnNames, table);
    }

    [Obsolete("use NotExistsIn")]
    public static SelectQuery NotExists(this SelectQuery query, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        return query.NotExistsIn(validationTableName, keyColumnNames);
    }

    [Obsolete("use NotExistsIn")]
    public static SelectQuery NotExists(this SelectQuery query, string tableName, IEnumerable<string> keyColumnNames, string validationTableName)
    {
        return query.NotExistsIn(validationTableName, keyColumnNames, tableName);
    }

    public static SelectQuery NotExistsIn(this SelectQuery query, FluentTable validationTable, string condition)
    {
        if (validationTable.IsCommonTable)
        {
            query.With(validationTable);
        }

        query.Where(() =>
        {
            var sq = new SelectQuery().From(validationTable);
            sq.Where(condition);
            return sq.ToNotExists();
        });

        return query;
    }

    public static SelectQuery NotExistsIn(this SelectQuery query, FluentTable validationTable, IEnumerable<string> validationcolumns, FluentTable table, IEnumerable<string> columns)
    {
        if (columns.Count() != validationcolumns.Count())
        {
            throw new ArgumentException("The number of elements in columns and validationcolumns must be the same.");
        }

        var columnMaps = columns.Zip(validationcolumns, (left, right) => (left, right));

        var lquery = new SelectQuery().From(table).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(validationTable).GetCurrentQuerySource();

        var condition = string.Join(" and ", columnMaps.Select(item => $"{rquery.GetColumn(item.right, isAliasIncluded: true)} = {lquery.GetColumn(item.left, isAliasIncluded: true)}"));

        var targetQuery = query.GetQuerySources()
                    .Where(x => x.HasTable(table.Alias, true))
                    .EnsureAny($"table:{table.Alias}")
                    .GetRootsByQuery()
                    .First()
                    .Query;

        targetQuery.NotExistsIn(validationTable, condition);

        return query;
    }

    /// <summary>
    /// Applies a NotExists query to the specified validation table based on the key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    public static SelectQuery NotExistsIn(this SelectQuery query, FluentTable validationTable, IEnumerable<string> keyColumnNames)
    {
        if (validationTable.IsCommonTable)
        {
            query.With(validationTable);
        }

        query.GetQuerySources()
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsByQuery()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery().From(validationTable);
                    keyColumnNames.ForEach(keyColumn => sq.Where($"{validationTable.Alias}.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToNotExists();
                });
            });

        return query;
    }

    /// <summary>
    /// Applies a NotExists query to the specified validation table based on the given table and key column names.
    /// </summary>
    /// <param name="query">The SelectQuery object to operate on.</param>
    /// <param name="validationTable">The FluentTable object used for validation.</param>
    /// <param name="keyColumnNames">A list of key column names used in the query.</param>
    /// <param name="table">The FluentTable object to operate on.</param>
    /// <returns>Returns the updated SelectQuery object.</returns>
    public static SelectQuery NotExistsIn(this SelectQuery query, FluentTable validationTable, IEnumerable<string> keyColumnNames, FluentTable table)
    {
        if (validationTable.IsCommonTable)
        {
            query.With(validationTable);
        }

        query.GetQuerySources()
            .Where(x => x.HasTable(table.Alias, true))
            .EnsureAny($"table:{table.Alias}")
            .Where(x => keyColumnNames.All(keyColumn => x.ColumnNames.Contains(keyColumn)))
            .EnsureAny($"columns:{string.Join(",", keyColumnNames)}")
            .GetRootsByQuery()
            .ForEach(qs =>
            {
                qs.Query.Where(() =>
                {
                    var sq = new SelectQuery().From(validationTable);
                    keyColumnNames.ForEach(keyColumn => sq.Where($"{validationTable.Alias}.{keyColumn} = {qs.Alias}.{keyColumn}"));
                    return sq.ToNotExists();
                });
            });

        return query;
    }

    public static SelectQuery NotExistsIn(this SelectQuery query, string validationTableName, IEnumerable<string> keyColumnNames)
    {
        query.AddNotExists(keyColumnNames, validationTableName);
        return query;
    }

    public static SelectQuery NotExistsIn(this SelectQuery query, string validationTableName, IEnumerable<string> keyColumnNames, string tableName)
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
