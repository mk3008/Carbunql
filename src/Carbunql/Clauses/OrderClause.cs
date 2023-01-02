using Carbunql.Values;
using System.Collections;

namespace Carbunql.Clauses;

public class OrderClause : SortableValueCollection, IQueryCommand
{
    public OrderClause(List<SortableItem> collection) : base(collection)
    {
    }

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!Collection.Any()) yield break;

        var clause = Token.Reserved(this, parent, "order by");
        yield return clause;

        foreach (var item in base.GetTokens(clause)) yield return item;
    }
}