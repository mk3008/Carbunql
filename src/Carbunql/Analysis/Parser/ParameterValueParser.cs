using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class ParameterValueParser
{
    public static bool IsParameterValue(string text)
    {
        if (text.StartsWith("@@")) return false;
        return text.StartsWith(new string[] { ":", "@", "?" });
    }

    public static ParameterValue Parse(ITokenReader r)
    {
        var v = r.Peek();
        if (!IsParameterValue(v)) throw new SyntaxException($"not a parameter.(text:{v})"); ;
        return new ParameterValue(r.Read());
    }
}
