using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class OperatableValue : IQueryCommand
{
    public OperatableValue(string @operator, ValueBase value)
    {
        Operator = @operator;
        Value = value;
    }

    public string Operator { get; init; }

    public ValueBase Value { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(Operator))
        {
            yield return Token.Reserved(this, parent, Operator);
        }
        foreach (var item in Value.GetTokens(parent)) yield return item;
    }
}