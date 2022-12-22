using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class LiteralValue : ValueBase
{
    public LiteralValue(string commandText)
    {
        CommandText = commandText;
    }

    public string CommandText { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return new Token(this, parent, CommandText);
    }
}