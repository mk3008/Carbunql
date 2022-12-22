using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;
using Carbunql.Core.Values;

namespace Carbunql.Analysis.Parser;

public static class ValuesClauseParser
{
    public static ValuesClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static ValuesClause Parse(TokenReader r)
    {
        var fn = () =>
        {
            if (!r.PeekRawToken().AreEqual(",")) return false;
            r.ReadToken(",");
            r.ReadToken("(");
            return true;
        };

        r.TryReadToken("values");
        r.ReadToken("(");

        var lst = new List<ValueCollection>();
        do
        {
            var (_, inner) = r.ReadUntilCloseBracket();
            lst.Add(ValueCollectionParser.Parse(inner));
        } while (fn());

        return new ValuesClause(lst);
    }
}