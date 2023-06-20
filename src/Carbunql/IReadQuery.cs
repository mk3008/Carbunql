using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql;

public interface IReadQuery : IQueryCommandable
{
	SelectClause? GetSelectClause();

	WithClause? GetWithClause();

	SelectQuery GetOrNewSelectQuery();

	IEnumerable<string> GetColumnNames();

	IEnumerable<SelectableTable> GetSelectableTables(bool cascade = false);

	IEnumerable<string> GetPhysicalTables();
}

public static class IReadQueryExtension
{
	public static ValueBase ToValue(this IReadQuery source)
	{
		return new QueryContainer(source);
	}
}