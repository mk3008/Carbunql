using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class NegativeValueParser
{
    public static bool IsNegativeValue(string text)
    {
        return text.IsEqualNoCase("not");
    }

    public static NegativeValue Parse(ITokenReader r)
    {
        r.Read("not");
        return new NegativeValue(ValueParser.Parse(r));
    }
}
