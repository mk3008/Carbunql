using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class PartitionClauseParser
{
	public static PartitionClause Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return new PartitionClause(ValueCollectionParser.ReadValues(r).ToList());
	}

	public static PartitionClause Parse(ITokenReader r)
	{
		return new PartitionClause(ValueCollectionParser.ReadValues(r).ToList());
	}
}