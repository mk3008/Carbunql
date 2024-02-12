using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public class TopParser
{
	public static TopClause Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static TopClause Parse(ITokenReader r)
	{
		r.Read("Top");
		var value = ValueParser.Parse(r);
		return new TopClause(value);
	}
}