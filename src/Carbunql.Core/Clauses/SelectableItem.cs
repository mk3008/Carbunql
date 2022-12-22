namespace Carbunql.Core.Clauses;

public class SelectableItem : IQueryCommand, ISelectable
{
    public SelectableItem(ValueBase value, string alias)
    {
        Value = value;
        Alias = alias;
    }

    public ValueBase Value { get; init; }

    public string Alias { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        if (!string.IsNullOrEmpty(Alias) && Alias != Value.GetDefaultName())
        {
            yield return Token.Reserved(this, parent, "as");
            yield return new Token(this, parent, Alias);
        }
    }
}