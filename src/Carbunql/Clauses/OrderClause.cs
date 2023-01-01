using Carbunql.Values;
using System.Collections;

namespace Carbunql.Clauses;

public class OrderClause : SortableValueCollection, IQueryCommand
{
    public OrderClause(List<SortableItem> collection) : base(collection)
    {
    }

    //public IEnumerable<Token> GetTokens(Token? parent)
    //{
    //    var clause = Token.Reserved(this, parent, "order by");
    //    yield return clause;

    //    var isFirst = true;
    //    foreach (var item in Collection)
    //    {
    //        if (isFirst)
    //        {
    //            isFirst = false;
    //        }
    //        else
    //        {
    //            yield return Token.Comma(this, clause);
    //        }
    //        foreach (var token in item.GetTokens(clause)) yield return token;
    //    }
    //}
}