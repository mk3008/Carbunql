using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class SortableCollectionParser
{
    public static SortableValueCollection Parse(string text)
    {
        using var r = new TokenReader(text);
        return new SortableValueCollection(ReadValues(r).ToList());
    }

    public static SortableValueCollection Parse(TokenReader r)
    {
        return new SortableValueCollection(ReadValues(r).ToList());
    }

    private static IEnumerable<SortableItem> ReadValues(TokenReader r)
    {
        do
        {
            if (r.PeekRawToken().AreEqual(",")) r.ReadToken();
            yield return SortableItemParser.Parse(r);
        }
        while (r.PeekRawToken().AreEqual(","));
    }
}