using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis.Parser;

public static class OrderClauseParser
{
    public static OrderClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return new OrderClause(ReadItems(r).ToList());
    }

    public static OrderClause Parse(TokenReader r)
    {
        return new OrderClause(ReadItems(r).ToList());
    }

    private static IEnumerable<SortableItem> ReadItems(TokenReader r)
    {
        do
        {
            r.TryReadToken(",");
            yield return SortableItemParser.Parse(r);
        }
        while (r.PeekRawToken().AreEqual(","));
    }
}