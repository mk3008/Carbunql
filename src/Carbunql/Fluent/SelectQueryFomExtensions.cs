namespace Carbunql.Fluent;

public static class SelectQueryFomExtensions
{
    public static SelectQuery From(this SelectQuery query, string table, string alias)
    {
        query.AddFrom(table, alias);
        return query;
    }
}
