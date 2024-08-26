namespace Carbunql.Fluent;

public static class SelectQueryOrderExtensions
{
    public static SelectQuery OrderBy(this SelectQuery query, string column)
    {
        query.AddOrder(column, (source, col) => $"{col}", true);
        return query;
    }

    public static SelectQuery OrderByDescending(this SelectQuery query, string column)
    {
        query.AddOrder(column, (source, col) => $"{col} desc", true);
        return query;
    }
}
