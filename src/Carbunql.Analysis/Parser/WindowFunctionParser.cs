using Carbunql.Core.Values;

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
        var dic = new Dictionary<string, ValueCollection>();

        using var r = new TokenReader(text);
        do
        {
            var token = r.ReadToken();
            var vs = ValueCollectionParser.Parse(r);
            dic.Add(token, vs);

        } while (r.PeekOrDefault() != null);

        var winfn = new WindowFunction();
        if (dic.ContainsKey("partition by")) winfn.PartitionBy = dic["partition by"];
        if (dic.ContainsKey("order by")) winfn.OrderBy = dic["order by"];

        return winfn;
    }
}