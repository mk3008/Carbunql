using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class LiteralValueParser
{
    public static bool IsLiteralValue(string text)
    {
        return (text.IsNumeric() || text.StartsWith("'") || text.IsEqualNoCase("true") || text.IsEqualNoCase("false"));
    }

    public static LiteralValue Parse(ITokenReader r)
    {
        return new LiteralValue(r.Read());
    }
}
