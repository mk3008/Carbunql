using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class WhenExpression : IQueryCommand
{
    public WhenExpression(ValueBase condition, ValueBase value)
    {
        Condition = condition;
        Value = value;
    }

    public WhenExpression(ValueBase value)
    {
        Value = value;
    }

    public ValueBase? Condition { get; init; }

    public ValueBase Value { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (Condition != null)
        {
            yield return Token.Reserved(this, parent, "when");
            foreach (var item in Condition.GetTokens(parent)) yield return item;
            yield return Token.Reserved(this, parent, "then");
            foreach (var item in Value.GetTokens(parent)) yield return item;
        }
        else
        {
            yield return Token.Reserved(this, parent, "else");
            foreach (var item in Value.GetTokens(parent)) yield return item;
        }
    }
}