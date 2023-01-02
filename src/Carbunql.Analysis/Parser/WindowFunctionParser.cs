using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class WindowFunctionParser
{
    public static WindowFunction Parse(TokenReader r)
    {
        r.ReadToken("(");
        var (_, inner) = r.ReadUntilCloseBracket();
        return Parse(inner);
    }

    public static WindowFunction Parse(string text)
    {
        var winfn = new WindowFunction();

        using var r = new TokenReader(text);
        do
        {
            var token = r.ReadToken();
            if (token.AreEqual("partition by"))
            {
                winfn.PartitionBy = PartitionClauseParser.Parse(r);
            }
            else if (token.AreEqual("order by"))
            {
                winfn.OrderBy = OrderClauseParser.Parse(r);
            }
        } while (r.PeekOrDefault() != null);

        return winfn;
    }
}