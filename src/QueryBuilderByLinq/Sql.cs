namespace QueryBuilderByLinq;

public static class Sql
{
	public static IQueryable<T> From<T>()
	{
		return Enumerable.Empty<T>().AsQueryable();
	}

}
