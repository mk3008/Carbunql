using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql;

public interface IReadQuery : IQueryCommandable
{
	SelectClause? GetSelectClause();

	ReadQuery GetQuery();
}

public static class IReadQueryExtension
{
	public static ValueBase ToValue(this IReadQuery source)
	{
		return new QueryContainer(source.GetQuery());
	}
}