namespace Carbunql.Fluent;

public static class SelectQuerySelectExtensions
{
    public static SelectQuery SelectAll(this SelectQuery query, string querySourceName)
    {
        query.AddSelectAll(querySourceName);
        return query;
    }

    public static SelectQuery Select(this SelectQuery query, string tableAlias, string column, string coluimnAlias = "")
    {
        query.AddSelect($"{tableAlias}.{column}", coluimnAlias);
        return query;
    }

    public static SelectQuery Select(this SelectQuery query, string tableAlias, IEnumerable<string> columnNames)
    {
        columnNames.ForEach(x => query.AddSelect($"{tableAlias}.{x}"));
        return query;
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
