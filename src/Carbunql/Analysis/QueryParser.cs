using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class QueryParser
{
    public static IReadQuery Parse(string text)
    {
        var r = new SqlTokenReader(text);
        var q = Parse(r);

        if (!r.Peek().IsEndToken())
        {
            throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
        }

        return q;
    }

    public static IReadQuery Parse(ITokenReader r)
    {
        var token = r.Peek();
        if (token.IsEqualNoCase("with")) return CTEQueryParser.Parse(r);
        if (token.IsEqualNoCase("select")) return SelectQueryParser.Parse(r);
        if (token.IsEqualNoCase("values")) return ValuesQueryParser.Parse(r);

        throw new NotSupportedException($"Token:{r.Peek()}");
    }
}