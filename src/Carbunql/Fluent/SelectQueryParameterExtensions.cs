namespace Carbunql.Fluent;

public static class SelectQueryParameterExtensions
{
    public static SelectQuery AddParameter(this SelectQuery query, string name, object? value)
    {
        var prm = new QueryParameter(name, value);
        query.Parameters.Add(prm);
        return query;
    }
}
