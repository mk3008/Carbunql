using Carbunql.Core.Extensions;

namespace Carbunql.Core.Clauses;

public class Relation : IQueryCommand
{
    public Relation(SelectableTable query, TableJoin types)
    {
        Table = query;
        TableJoin = types;
    }

    public Relation(SelectableTable query, TableJoin types, ValueBase condition)
    {
        Table = query;
        TableJoin = types;
        Condition = condition;
    }

    public TableJoin TableJoin { get; init; }

    public ValueBase? Condition { get; set; }

    public SelectableTable Table { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, TableJoin.ToCommandText());
        foreach (var item in Table.GetTokens(parent)) yield return item;

        if (Condition != null)
        {
            yield return Token.Reserved(this, parent, "on");
            foreach (var item in Condition.GetTokens(parent)) yield return item;
        }
    }
}