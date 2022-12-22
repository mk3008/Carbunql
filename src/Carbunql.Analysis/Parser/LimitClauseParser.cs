using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis.Parser;

public static class LimitClauseParser
{
    public static LimitClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static LimitClause Parse(TokenReader r)
    {
        var condition = ParseItems(r).ToList();
        if (r.TryReadToken("offset") != null)
        {
            var offset = ValueParser.Parse(r);
            return new LimitClause(condition) { Offset = offset };
        }
        return new LimitClause(condition);
    }

    private static IEnumerable<ValueBase> ParseItems(TokenReader r)
    {
        do
        {
            if (r.PeekRawToken().AreEqual(",")) r.ReadToken();
            yield return ValueParser.Parse(r);
        }
        while (r.PeekRawToken().AreEqual(","));
    }
}