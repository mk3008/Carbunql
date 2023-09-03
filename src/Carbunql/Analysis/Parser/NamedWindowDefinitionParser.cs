using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class NamedWindowDefinitionParser
{
	public static NamedWindowDefinition Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static NamedWindowDefinition Parse(ITokenReader r)
	{
		var alias = r.Read();
		r.Read("as");
		r.Read("(");
		var w = WindowDefinitionParser.Parse(r);
		r.Read(")");
		return new NamedWindowDefinition(alias, w);
	}
}