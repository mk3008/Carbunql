using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class ReadQueryParser
{
    public static ReadQuery Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static ReadQuery Parse(TokenReader r)
    {
        if (r.PeekRawToken().AreEqual("select")) return SelectQueryParser.Parse(r);
        if (r.PeekRawToken().AreEqual("values")) return ValuesQueryParser.Parse(r);

        throw new NotSupportedException();
    }
}