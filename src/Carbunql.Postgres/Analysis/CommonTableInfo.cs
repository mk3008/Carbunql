namespace Carbunql.Postgres.Analysis;

public class CommonTableInfo
{
	public CommonTableInfo(IQueryable query, string alias)
	{
		Alias = alias;
		Query = query;
	}

	public string Alias { get; private set; }

	public IQueryable Query { get; private set; }
}
