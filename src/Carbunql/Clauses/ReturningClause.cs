using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

public class ReturningClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
    public ReturningClause(ValueBase value)
    {
        if (value is ValueCollection collection)
        {
            foreach (var item in collection)
            {
                Items.Add(item);
            }
        }
        else
        {
            Items.Add(value);
        }
    }

    public ReturningClause(IEnumerable<ValueBase> values)
    {
        foreach (var item in values)
        {
            Items.Add(item);
        }
    }

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "returning");
        yield return t;
        foreach (var item in base.GetTokens(t)) yield return item;
    }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }
}