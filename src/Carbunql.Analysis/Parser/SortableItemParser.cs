using Carbunql.Core;
using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SortableItemParser
{
    public static SortableItem Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SortableItem Parse(TokenReader r)
    {
        var breaktokens = TokenReader.BreakTokens;

        var v = ValueParser.Parse(r);
        var isasc = true;

        if (r.PeekRawToken().AreContains(breaktokens))
        {
            return new SortableItem(v);
        }

        if (r.PeekRawToken().AreEqual("asc"))
        {
            r.ReadToken("asc");
            isasc = true;
        }
        else if (r.PeekRawToken().AreEqual("desc"))
        {
            r.ReadToken("desc");
            isasc = false;
        }

        if (r.PeekRawToken().AreEqual("nulls"))
        {
            var t = r.ReadToken("nulls");
            if (t.AreEqual("nulls first"))
            {
                return new SortableItem(v, isasc, NullSort.First);
            }
            else if (t.AreEqual("nulls last"))
            {
                return new SortableItem(v, isasc, NullSort.Last);
            }
            throw new NotSupportedException();
        }
        return new SortableItem(v, isasc, NullSort.Undefined);
    }
}