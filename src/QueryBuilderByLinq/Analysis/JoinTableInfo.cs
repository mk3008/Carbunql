namespace QueryBuilderByLinq.Analysis;

public class JoinTableInfo
{
	public JoinTableInfo(IQueryable query, string alias, string relation)
	{
		Alias = alias;
		Query = query;
		Relation = relation;
	}

	public JoinTableInfo(string physicalName, string alias, string relation)
	{
		Alias = alias;
		PhysicalName = physicalName;
		Relation = relation;
	}

	public string Alias { get; private set; }

	public string? PhysicalName { get; private set; }

	public IQueryable? Query { get; private set; }

	public string Relation { get; private set; }
}
