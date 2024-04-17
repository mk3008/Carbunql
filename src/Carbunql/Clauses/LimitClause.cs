using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class LimitClause : IQueryCommandable
{
    public LimitClause()
    {
        Condition = null!;
    }

    public LimitClause(string text)
    {
        Condition = new LiteralValue(text);
    }

    public LimitClause(ValueBase item)
    {
        Condition = item;
    }

    public LimitClause(List<ValueBase> conditions)
    {
        var lst = new ValueCollection();
        conditions.ForEach(x => lst.Add(x));
        Condition = lst;
    }

    public ValueBase Condition { get; init; }

    public ValueBase? Offset { get; set; }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Condition.GetInternalQueries())
        {
            yield return item;
        }
        if (Offset != null)
        {
            foreach (var item in Offset.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Condition.GetPhysicalTables())
        {
            yield return item;
        }
        if (Offset != null)
        {
            foreach (var item in Offset.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }


    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Condition.GetCommonTables())
        {
            yield return item;
        }
        if (Offset != null)
        {
            foreach (var item in Offset.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Condition.GetParameters())
        {
            yield return item;
        }
        var q = Offset?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "limit");
        yield return clause;

        foreach (var item in Condition.GetTokens(clause)) yield return item;
        if (Offset != null)
        {
            yield return Token.Reserved(this, clause, "offset");
            foreach (var item in Offset.GetTokens(clause)) yield return item;
        }
    }
}