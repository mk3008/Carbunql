using Carbunql.Core.Extensions;
using Carbunql.Core.Values;

namespace Carbunql.Analysis.Parser;

public static class FunctionValueParser
{
    public static FunctionValue Parse(string text, string functionName)
    {
        using var r = new TokenReader(text);
        return Parse(r, functionName);
    }

    public static FunctionValue Parse(TokenReader r, string functionName)
    {
        r.ReadToken("(");
        var (_, argstext) = r.ReadUntilCloseBracket();
        var arg = ValueCollectionParser.Parse(argstext);

        if (!r.PeekRawToken().AreEqual("over"))
        {
            return new FunctionValue(functionName, arg);
        }

        r.ReadToken("over");
        var winfn = WindowFunctionParser.Parse(r);
        return new FunctionValue(functionName, arg, winfn);
    }
}