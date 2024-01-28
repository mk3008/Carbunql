using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public class WhereClauseParser
{
	public static WhereClause Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static WhereClause Parse(ITokenReader r)
	{
		var val = ValueParser.Parse(r);
		var where = new WhereClause(val);
		return where;
	}
}