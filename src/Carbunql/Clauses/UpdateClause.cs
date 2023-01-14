namespace Carbunql.Clauses;

public class UpdateClause : IQueryCommand
{
    public UpdateClause(SelectableTable table)
    {
        Table = new SelectableTable(table.Table, table.Alias);
    }

    public SelectableTable Table { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "update");
        yield return t;
        foreach (var item in Table.GetTokens(t)) yield return item;
    }
}