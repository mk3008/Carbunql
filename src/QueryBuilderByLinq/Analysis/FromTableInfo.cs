using Carbunql;
using Carbunql.Clauses;

namespace QueryBuilderByLinq.Analysis;

public class FromTableInfo
{
	public FromTableInfo(SelectQuery query, string alias)
	{
		Alias = alias;
		Query = query;
	}

	public FromTableInfo(SelectableTable table, string alias)
	{
		Alias = alias;
		Table = table;
	}

	public FromTableInfo(string physicalName, string alias)
	{
		Alias = alias;
		PhysicalName = physicalName;
	}

	public string Alias { get; private set; }

	public string? PhysicalName { get; private set; }

	public SelectableTable? Table { get; private set; }

	public SelectQuery? Query { get; private set; }
}
