namespace Carbunql.Fluent;

public static class SelectQueryParameterExtensions
{
    public static SelectQuery AddParameter(this SelectQuery query, string name, object? value)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return query;
    }

    public static SelectQuery FluentParameter(this SelectQuery query, string name, object? value)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return query;
    }

    public static SelectQuery FluentParameter(this SelectQuery query, string name, object? value, Func<string, SelectQuery> func)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return func(prm.ParameterName);
    }
}
