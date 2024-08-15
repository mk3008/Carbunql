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
}
