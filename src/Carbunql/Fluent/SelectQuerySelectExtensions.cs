namespace Carbunql.Fluent;

public static class SelectQuerySelectExtensions
{
    /// <summary>
    /// Adds all columns of the specified table to the SELECT clause.
    /// If column aliases are present, they are used; otherwise, the table's column names are used.
    /// If no column names are available, a wildcard (*) is used.
    /// </summary>
    /// <param name="query">The <see cref="SelectQuery"/> object to which the columns will be added.</param>
    /// <param name="table">The <see cref="FluentTable"/> object that contains the column information.</param>
    /// <returns>The <see cref="SelectQuery"/> object with the columns added.</returns>
    public static SelectQuery SelectAll(this SelectQuery query, FluentTable table)
    {
        if (table.ColumnAliases.Any())
        {
            table.ColumnAliases.ForEach(x => query.AddSelect($"{table.Alias}.{x}"));
        }
        else if (table.Table.GetColumnNames().Any())
        {
            table.Table.GetColumnNames().ForEach(x => query.AddSelect($"{table.Alias}.{x}"));
        }
        else
        {
            query.AddSelect($"{table.Alias}.*");
        }
        return query;
    }

    public static SelectQuery SelectAll(this SelectQuery query, string querySourceName)
    {
        query.AddSelectAll(querySourceName);
        return query;
    }

    /// <summary>
    /// Modifies the SELECT clause to include only the columns with the specified alias names, removing all other columns.
    /// </summary>
    /// <param name="query">The <see cref="SelectQuery"/> object to be modified.</param>
    /// <param name="columnAliasNames">A list of column alias names to retain in the SELECT clause.</param>
    /// <returns>The modified <see cref="SelectQuery"/> object.</returns>
    /// <exception cref="NullReferenceException">Thrown if the SELECT clause (<see cref="query.SelectClause"/>) is null.</exception>
    public static SelectQuery SelectOnlyByNames(this SelectQuery query, IEnumerable<string> columnAliasNames)
    {
        if (query.SelectClause == null) throw new NullReferenceException(nameof(query.SelectClause));
        query.SelectClause.FilterInColumns(columnAliasNames);
        return query;
    }

    public static SelectQuery Select(this SelectQuery query, string tableAlias, string column, string columnAlias = "")
    {
        query.AddSelect($"{tableAlias}.{column}", columnAlias);
        return query;
    }

    public static SelectQuery Select(this SelectQuery query, string tableAlias, IEnumerable<string> columnNames)
    {
        columnNames.ForEach(x => query.AddSelect($"{tableAlias}.{x}"));
        return query;
    }

    public static SelectQuery Select(this SelectQuery query, FluentTable table, string column, string columnAlias = "")
    {
        return query.Select(table.Alias, column, columnAlias);
    }

    public static SelectQuery Select(this SelectQuery query, FluentTable table, IEnumerable<string> columnNames)
    {
        return query.Select(table.Alias, columnNames);
    }

    public static SelectQuery SelectValue(this SelectQuery query, string value, string alias)
    {
        query.AddSelect(value, alias);
        return query;
    }

    public static SelectQuery Greatest(this SelectQuery query, string columnAlias, object value)
    {
        query.OverrideSelect(columnAlias, (source, col) => $"greatest({col}, {ToValueText(value)})");
        return query;
    }

    public static SelectQuery Greatest(this SelectQuery query, FluentTable cte, string columnAlias, object value)
    {
        return query.Greatest(cte.Alias, columnAlias, value);
    }

    public static SelectQuery Greatest(this SelectQuery query, string tableName, string columnAlias, object value)
    {
        query.OverrideSelect(tableName, columnAlias, (source, col) => $"greatest({col}, {ToValueText(value)})");
        return query;
    }

    public static SelectQuery Least(this SelectQuery query, string columnAlias, object value)
    {
        query.OverrideSelect(columnAlias, (source, col) => $"least({col}, {ToValueText(value)})");
        return query;
    }

    public static SelectQuery Least(this SelectQuery query, string tableName, string columnAlias, object value)
    {
        query.OverrideSelect(tableName, columnAlias, (source, col) => $"least({col}, {ToValueText(value)})");
        return query;
    }

    public static SelectQuery Least(this SelectQuery query, FluentTable cte, string columnAlias, object value)
    {
        return query.Least(cte.Alias, columnAlias, value);
    }

    public static SelectQuery ReverseSign(this SelectQuery query, string columnAlias)
    {
        query.OverrideSelect(columnAlias, (source, col) => $"({col}) * -1");
        return query;
    }

    public static SelectQuery ReverseSign(this SelectQuery query, string tableName, string columnAlias)
    {
        query.OverrideSelect(tableName, columnAlias, (source, col) => $"({col}) * -1");
        return query;
    }

    public static SelectQuery ReverseSign(this SelectQuery query, FluentTable table, string columnAlias)
    {
        return query.ReverseSign(table.Alias, columnAlias);
    }

    public static SelectQuery ReverseSign(this SelectQuery query, IEnumerable<string> columnAliases)
    {
        columnAliases.ForEach(x => ReverseSign(query, x));
        return query;
    }

    public static SelectQuery ReverseSign(this SelectQuery query, string tableName, IEnumerable<string> columnAliases)
    {
        columnAliases.ForEach(x => ReverseSign(query, tableName, x));
        return query;
    }

    public static SelectQuery ReverseSign(this SelectQuery query, FluentTable table, IEnumerable<string> columnAliases)
    {
        return query.ReverseSign(table.Alias, columnAliases);
    }

    private static string ToValueText(object value)
    {
        if (value is string s)
        {
            if (!SelectQueryWhereExtensions.ParameterSymbols.Where(s.StartsWith).Any())
            {
                throw new InvalidOperationException("The string type must be parameterized.");
            }
            return s;
        }
        else if (value is DateTime)
        {
            return $"'{value.ToString()}'";
        }
        else
        {
            return value.ToString()!;
        }
    }
}
