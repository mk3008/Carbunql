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

    public static SelectQuery Union(this SelectQuery query, SelectQuery additionalQuery)
    {
        query.AddSelectQuery("union", _ => additionalQuery);
        return query;
    }

    public static SelectQuery UnionAll(this SelectQuery query, SelectQuery additionalQuery)
    {
        query.AddSelectQuery("union all", _ => additionalQuery);
        return query;
    }

    public static SelectQuery Intersect(this SelectQuery query, SelectQuery additionalQuery)
    {
        query.AddSelectQuery("intersect", _ => additionalQuery);
        return query;
    }

    public static SelectQuery IntersectAll(this SelectQuery query, SelectQuery additionalQuery)
    {
        query.AddSelectQuery("intersect all", _ => additionalQuery);
        return query;
    }

    public static SelectQuery Except(this SelectQuery query, SelectQuery additionalQuery)
    {
        query.AddSelectQuery("except", _ => additionalQuery);
        return query;
    }

    public static SelectQuery ExceptAll(this SelectQuery query, SelectQuery additionalQuery)
    {
        query.AddSelectQuery("except all", _ => additionalQuery);
        return query;
    }

    public static SelectQuery Minus(this SelectQuery query, SelectQuery additionalQuery)
    {
        query.AddSelectQuery("minus", _ => additionalQuery);
        return query;
    }
}
