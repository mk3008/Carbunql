using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class WithClauseParser
{
	public static WithClause Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static WithClause Parse(ITokenReader r)
	{
		r.ReadToken("with");

		var recursive = r.TryReadToken("recursive") != null ? true : false;
		return new WithClause(ParseCommonTables(r).ToList()) { HasRecursiveKeyword = recursive };
	}

	private static IEnumerable<CommonTable> ParseCommonTables(ITokenReader r)
	{
		do
		{
			if (r.PeekRawToken().AreEqual(",")) r.ReadToken();
			yield return CommonTableParser.Parse(r);
		}
		while (r.PeekRawToken().AreEqual(","));
	}
}