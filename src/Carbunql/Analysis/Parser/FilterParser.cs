using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class FilterParser
{
	public static Filter Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static Filter ParseAsInner(ITokenReader r)
	{
		r.Read("(");
		using var ir = new BracketInnerTokenReader(r);
		var v = Parse(ir);
		return v;
	}

	public static Filter Parse(ITokenReader r)
	{
		r.ReadOrDefault("(");
		r.Read("where");

		var filter = new Filter() { WhereClause = WhereClauseParser.Parse(r) };
		r.ReadOrDefault(")");
		return filter;	
	}
}