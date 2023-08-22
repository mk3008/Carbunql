using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql;

[MessagePack.Union(0, typeof(ReadQuery))]
public interface IReadQuery : IQueryCommandable
{
	SelectClause? GetSelectClause();

	WithClause? GetWithClause();

	SelectQuery GetOrNewSelectQuery();

	IEnumerable<string> GetColumnNames();
}

public static class IReadQueryExtension
{
	public static ValueBase ToValue(this IReadQuery source)
	{
		return new QueryContainer(source);
	}
}