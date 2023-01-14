using Carbunql.Extensions;

namespace Carbunql.Clauses;

public class CreateTableClause : IQueryCommand
{
    public CreateTableClause()
    {
    }

    public CreateTableClause(TableBase table)
    {
        Table = table;
    }

    public bool IsTemporary { get; set; } = true;

    public TableBase? Table { get; set; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (Table == null) throw new Exception();

        Token clause = GetClauseToken(parent);
        yield return clause;

        foreach (var item in Table.GetTokens(clause)) yield return item;
    }

    private Token GetClauseToken(Token? parent)
    {
        if (IsTemporary)
        {
            return Token.Reserved(this, parent, "create temporary table");
        }
        return Token.Reserved(this, parent, "create table");
    }
}