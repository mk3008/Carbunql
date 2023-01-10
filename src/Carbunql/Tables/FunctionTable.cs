using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Tables;

public class FunctionTable : TableBase
{
    public FunctionTable(string name)
    {
        Name = name;
        Argument = new ValueCollection();
    }

    public FunctionTable(string name, ValueBase args)
    {
        Name = name;
        Argument = new ValueCollection
        {
            args
        };
    }

    public string Name { get; init; }

    public ValueCollection Argument { get; init; }

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, Name);

        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Argument.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }
}