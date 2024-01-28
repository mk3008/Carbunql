using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public class GroupClauseParser
{
	public static GroupClause Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static GroupClause Parse(ITokenReader r)
	{
		var vals = ValueCollectionParser.Parse(r);
		var group = new GroupClause(vals);
		return group;
	}
}