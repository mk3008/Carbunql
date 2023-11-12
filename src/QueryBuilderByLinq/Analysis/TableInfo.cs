using Carbunql;
using Carbunql.Clauses;
using Carbunql.Tables;

namespace QueryBuilderByLinq.Analysis;

public class TableInfo
{
	public TableInfo(SelectableTable table)
	{
		Alias = table.Alias;
		Table = table;
	}

	public string Alias { get; private set; }

	public SelectableTable? Table { get; private set; }

	public SelectableTable ToSelectable()
	{
		if (Table != null) return Table;
		throw new InvalidProgramException();
	}
}
