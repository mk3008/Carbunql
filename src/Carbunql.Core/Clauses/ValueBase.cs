using Carbunql.Core.Values;

namespace Carbunql.Core.Clauses;

public abstract class ValueBase : IQueryCommand
{
    public string? Sufix { get; set; }

    public virtual string GetDefaultName() => string.Empty;

    public OperatableValue? OperatableValue { get; private set; }

    public ValueBase AddOperatableValue(string @operator, ValueBase value)
    {
        if (OperatableValue != null) throw new InvalidOperationException();
        OperatableValue = new OperatableValue(@operator, value);
        return value;
    }

    public abstract IEnumerable<Token> GetCurrentTokens(Token? parent);

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in GetCurrentTokens(parent)) yield return item;

        if (Sufix != null) yield return Token.Reserved(this, parent, Sufix);
        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.GetTokens(parent)) yield return item;
        }
    }
}