using Carbunql.Analysis.Parser;
using Carbunql.Core;
using Carbunql.Core.Extensions;

namespace Carbunql.Analysis;

public static class QueryParser
{
    public static QueryBase Parse(string text)
    {
        using var r = new TokenReader(text);
        if (r.TryReadToken("with") != null) WithClauseParser.Parse(r);
        if (r.PeekRawToken().AreEqual("select")) return SelectQueryParser.Parse(r);
        if (r.PeekRawToken().AreEqual("values")) return ValuesQueryParser.Parse(r);

        throw new NotSupportedException();
    }
}
