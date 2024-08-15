namespace Carbunql.Fluent;

public static class SelectQueryOperateExtensions
{
    public static SelectQuery Union(this SelectQuery query, Func<SelectQuery> builder)
    {
        query.AddSelectQuery("union", _ => builder());
        return query;
    }

    public static SelectQuery UnionAll(this SelectQuery query, Func<SelectQuery> builder)
    {
        query.AddSelectQuery("union all", _ => builder());
        return query;
    }

    public static SelectQuery Intersect(this SelectQuery query, Func<SelectQuery> builder)
    {
        query.AddSelectQuery("intersect", _ => builder());
        return query;
    }

    public static SelectQuery IntersectAll(this SelectQuery query, Func<SelectQuery> builder)
    {
        query.AddSelectQuery("intersect all", _ => builder());
        return query;
    }

    public static SelectQuery Except(this SelectQuery query, Func<SelectQuery> builder)
    {
        query.AddSelectQuery("except", _ => builder());
        return query;
    }

    public static SelectQuery ExceptAll(this SelectQuery query, Func<SelectQuery> builder)
    {
        query.AddSelectQuery("except all", _ => builder());
        return query;
    }

    public static SelectQuery Minus(this SelectQuery query, Func<SelectQuery> builder)
    {
        query.AddSelectQuery("minus", _ => builder());
        return query;
    }

    public static SelectQuery Union(this SelectQuery query, Func<SelectQuery, SelectQuery> builder)
    {
        query.AddSelectQuery("union", builder);
        return query;
    }

    public static SelectQuery UnionAll(this SelectQuery query, Func<SelectQuery, SelectQuery> builder)
    {
        query.AddSelectQuery("union all", builder);
        return query;
    }

    public static SelectQuery Intersect(this SelectQuery query, Func<SelectQuery, SelectQuery> builder)
    {
        query.AddSelectQuery("intersect", builder);
        return query;
    }

    public static SelectQuery IntersectAll(this SelectQuery query, Func<SelectQuery, SelectQuery> builder)
    {
        query.AddSelectQuery("intersect all", builder);
        return query;
    }

    public static SelectQuery Except(this SelectQuery query, Func<SelectQuery, SelectQuery> builder)
    {
        query.AddSelectQuery("except", builder);
        return query;
    }

    public static SelectQuery ExceptAll(this SelectQuery query, Func<SelectQuery, SelectQuery> builder)
    {
        query.AddSelectQuery("except all", builder);
        return query;
    }

    public static SelectQuery Minus(this SelectQuery query, Func<SelectQuery, SelectQuery> builder)
    {
        query.AddSelectQuery("minus", builder);
        return query;
    }
}
