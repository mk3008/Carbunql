using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis.Parser;

public static class WithClauseParser
{
    public static WithClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static WithClause Parse(TokenReader r)
    {
        var recursive = r.TryReadToken("recursive") != null ? true : false;
        return new WithClause(ParseCommonTables(r).ToList()) { HasRecursiveKeyword = recursive };
    }

    private static IEnumerable<CommonTable> ParseCommonTables(TokenReader r)
    {
        do
        {
            if (r.PeekRawToken().AreEqual(",")) r.ReadToken();
            yield return CommonTableParser.Parse(r);
        }
        while (r.PeekRawToken().AreEqual(","));
    }
}