namespace Carbunql.Postgres.Linq;

public class CommonTableInfo
{
    public CommonTableInfo(IQueryable query, string alias)
    {
        Alias = alias;
        Query = query.ToSelectQuery();
    }

    public CommonTableInfo(SelectQuery query, string alias)
    {
        Alias = alias;
        Query = query;
    }


    public string Alias { get; private set; }

    //public IQueryable Query { get; private set; }
    public SelectQuery Query { get; private set; }
}
