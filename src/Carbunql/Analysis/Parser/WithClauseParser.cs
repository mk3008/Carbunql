using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class WithClauseParser
{
	public static WithClause Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static WithClause Parse(ITokenReader r)
	{
		r.Read("with");

		var recursive = r.ReadOrDefault("recursive") != null ? true : false;
		return new WithClause(ParseCommonTables(r).ToList()) { HasRecursiveKeyword = recursive };
	}

	private static IEnumerable<CommonTable> ParseCommonTables(ITokenReader r)
	{
		do
		{
			if (r.Peek().IsEqualNoCase(",")) r.Read();
			yield return CommonTableParser.Parse(r);
		}
		while (r.Peek().IsEqualNoCase(","));
	}
}