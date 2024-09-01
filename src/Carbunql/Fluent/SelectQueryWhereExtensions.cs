using Carbunql.Building;
using Carbunql.Fluent;

namespace Carbunql.Fluent;

public static class SelectQueryWhereExtensions
{
    internal static char[] ParameterSymbols = { '@', ':', '$' };

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
}
