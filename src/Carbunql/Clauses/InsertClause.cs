namespace Carbunql.Clauses;

public class InsertClause : IQueryCommand
{
    public InsertClause(SelectableTable table)
    {
        Table = table;
    }

    public SelectableTable Table { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "insert into");
        yield return t;
        foreach (var item in Table.GetTokens(t)) yield return item;
    }
}