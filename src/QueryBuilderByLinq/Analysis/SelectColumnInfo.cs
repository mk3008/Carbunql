using Carbunql.Clauses;

namespace QueryBuilderByLinq.Analysis;

public class SelectColumnInfo
{
	public SelectColumnInfo(ValueBase value, string alias)
	{
		Value = value;
		Alias = alias;
	}

	public string Alias { get; private set; }

	public ValueBase Value { get; private set; }
}
