using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class LikeExpression : ValueBase
{
    public LikeExpression(ValueBase value, ValueBase argument)
    {
        Value = value;
        Argument = argument;
    }

    public ValueBase Value { get; init; }

    public ValueBase Argument { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "like");
        foreach (var item in Argument.GetTokens(parent)) yield return item;
    }
}