using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class PartitionClauseParser
{
	public static PartitionClause Parse(string text)
	{
		using var r = new TokenReader(text);
		return new PartitionClause(ValueCollectionParser.ReadValues(r).ToList());
	}

	public static PartitionClause Parse(ITokenReader r)
	{
		return new PartitionClause(ValueCollectionParser.ReadValues(r).ToList());
	}
}