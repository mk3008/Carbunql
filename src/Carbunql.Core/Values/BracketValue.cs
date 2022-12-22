using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class BracketValue : ValueBase
{
    public BracketValue(ValueBase inner)
    {
        Inner = inner;
    }

    public ValueBase Inner { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (Inner == null) yield break;

        var bracket = Token.ExpressionBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Inner.GetTokens(bracket)) yield return item;
        yield return Token.ExpressionBracketEnd(this, parent);
    }
}