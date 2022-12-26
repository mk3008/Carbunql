using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class QueryParser
{
    public static QueryBase Parse(string text)
    {
        using var r = new TokenReader(text);
        WithClause? w = null;
        if (r.TryReadToken("with") != null) w = WithClauseParser.Parse(r);

        if (r.PeekRawToken().AreEqual("select")) return SelectQueryParser.Parse(r, w);
        if (r.PeekRawToken().AreEqual("values")) return ValuesQueryParser.Parse(r, w);

        throw new NotSupportedException();
    }
}
