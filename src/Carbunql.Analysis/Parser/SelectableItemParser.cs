using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectableItemParser
{
    public static SelectableItem Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectableItem Parse(TokenReader r)
    {
        var breaktokens = TokenReader.BreakTokens;

        var v = ValueParser.Parse(r);
        r.TryReadToken("as");

        if (r.PeekRawToken().AreContains(breaktokens))
        {
            return new SelectableItem(v, v.GetDefaultName());
        }

        return new SelectableItem(v, r.ReadToken());
    }
}