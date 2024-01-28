using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class OverClauseParser
{
	public static OverClause Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static OverClause ParseAsInner(ITokenReader r)
	{
		r.Read("(");
		using var ir = new BracketInnerTokenReader(r);
		var v = Parse(ir);
		return v;
	}

	public static OverClause Parse(ITokenReader r)
	{
		var clause = new OverClause();
		clause.WindowDefinition = WindowDefinitionParser.Parse(r);
		return clause;
	}
}