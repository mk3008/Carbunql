namespace Carbunql.Postgres;

public class SelectQuery<T> : SelectQuery
{
    public SelectQuery() : base()
    {
    }

    public SelectQuery(string query) : base(query)
    {
    }
}
