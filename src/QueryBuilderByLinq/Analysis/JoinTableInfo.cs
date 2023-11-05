using Carbunql.Clauses;

namespace QueryBuilderByLinq.Analysis;

public class JoinTableInfo
{
	public JoinTableInfo(TableInfo tableInfo, string relation)
	{
		TableInfo = tableInfo;
		Relation = relation;
	}

	public JoinTableInfo(TableInfo tableInfo, string relation, ValueBase condition)
	{
		TableInfo = tableInfo;
		Relation = relation;
		Condition = condition;
	}

	public TableInfo TableInfo { get; set; }

	public string Relation { get; private set; }

	public ValueBase? Condition { get; set; }
}
