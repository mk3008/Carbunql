namespace Carbunql.Core.Clauses;

public class FromClause : IQueryCommand
{
    public FromClause(SelectableTable root)
    {
        Root = root;
    }

    public SelectableTable Root { get; init; }

    public List<Relation>? Relations { get; set; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "from");

        yield return clause;
        foreach (var item in Root.GetTokens(clause)) yield return item;

        if (Relations == null) yield break;

        foreach (var item in Relations)
        {
            foreach (var token in item.GetTokens(clause)) yield return token;
        }
    }
}