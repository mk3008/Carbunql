using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class WindowClauseParser
{
	public static WindowClause Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static WindowClause Parse(ITokenReader r)
	{
		r.ReadOrDefault("window");

		return new WindowClause(ParseNamedWindowDefinitions(r).ToList());
	}

	private static IEnumerable<NamedWindowDefinition> ParseNamedWindowDefinitions(ITokenReader r)
	{
		do
		{
			if (r.Peek().IsEqualNoCase(",")) r.Read();
			yield return NamedWindowDefinitionParser.Parse(r);
		}
		while (r.Peek().IsEqualNoCase(","));
	}
}