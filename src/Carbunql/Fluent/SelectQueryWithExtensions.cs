using Carbunql.Building;

namespace Carbunql.Fluent;

public static class SelectQueryWithExtensions
{
    public static SelectQuery With(this SelectQuery sq, string query, string alias)
    {
        sq.With(query).As(alias);
        return sq;
    }

    public static SelectQuery With(this SelectQuery sq, string query, string alias, Materialized type)
    {
        var ct = sq.With(query).As(alias);
        ct.Materialized = type;
        return sq;
    }

    public static SelectQuery With(this SelectQuery sq, SelectQuery query, string alias)
    {
        sq.With(query).As(alias);
        return sq;
    }

    public static SelectQuery With(this SelectQuery sq, SelectQuery query, string alias, Materialized type)
    {
        var ct = sq.With(query).As(alias);
        ct.Materialized = type;
        return sq;
    }
}
