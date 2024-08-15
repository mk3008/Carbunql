namespace Carbunql.Fluent;

public static class SelectQueryOrderExtensions
{
    public static SelectQuery OrderBy(this SelectQuery query, string column)
    {
        query.AddOrder(column, (source, col) => $"{source.Alias}.{col}");
        return query;
    }

    public static SelectQuery OrderByDescending(this SelectQuery query, string column)
    {
        query.AddOrder(column, (source, col) => $"{source.Alias}.{col} desc");
        return query;
    }
}
