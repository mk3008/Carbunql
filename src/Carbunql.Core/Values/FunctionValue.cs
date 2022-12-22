using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class FunctionValue : ValueBase
{
    public FunctionValue(string name, string arg)
    {
        Name = name;
        Argument = new ValueCollection(arg);
    }

    public FunctionValue(string functionName, ValueCollection args)
    {
        Name = functionName;
        Argument = args;
    }

    public FunctionValue(string functionName, ValueCollection args, WindowFunction winfn)
    {
        Name = functionName;
        Argument = args;
        WindowFunction = winfn;
    }

    public string Name { get; init; }

    public ValueCollection? Argument { get; init; }

    public WindowFunction? WindowFunction { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, Name);

        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        if (Argument != null)
        {
            foreach (var item in Argument.GetTokens(bracket)) yield return item;
        }
        yield return Token.ReservedBracketEnd(this, parent);

        if (WindowFunction != null)
        {
            foreach (var item in WindowFunction.GetTokens(parent)) yield return item;
        }
    }
}