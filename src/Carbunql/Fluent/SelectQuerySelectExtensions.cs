namespace Carbunql.Fluent;

public static class SelectQuerySelectExtensions
{
    public static SelectQuery SelectAll(this SelectQuery query, string querySourceName)
    {
        query.AddSelectAll(querySourceName);
        return query;
    }

    public static SelectQuery Select(this SelectQuery query, string column)
    {
        query.AddSelect(column);
        return query;
    }

    public static SelectQuery Select(this SelectQuery query, string column, string alias)
    {
        query.AddSelect(column, alias);
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
