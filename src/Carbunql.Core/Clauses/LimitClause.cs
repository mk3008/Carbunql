using Carbunql.Core.Values;

namespace Carbunql.Core.Clauses;

public class LimitClause : IQueryCommand
{
    public LimitClause(string text)
    {
        Conditions.Add(new LiteralValue(text));
    }

    public LimitClause(ValueBase item)
    {
        Conditions.Add(item);
    }

    public LimitClause(List<ValueBase> conditions)
    {
        conditions.ForEach(x => Conditions.Add(x));
    }

    public ValueCollection Conditions { get; init; } = new();

    public ValueBase? Offset { get; set; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "limit");
        yield return clause;

        foreach (var item in Conditions.GetTokens(clause)) yield return item;
        if (Offset != null)
        {
            yield return Token.Reserved(this, clause, "offset");
            foreach (var item in Offset.GetTokens(clause)) yield return item;
        }
    }
}