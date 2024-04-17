using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class SortableItem : IQueryCommandable
{
    public SortableItem(ValueBase value, bool isAscending = true, NullSort nullSort = NullSort.Undefined)
    {
        Value = value;
        IsAscending = isAscending;
        NullSort = nullSort;
    }

    public ValueBase Value { get; init; }

    public bool IsAscending { get; set; } = true;

    public NullSort NullSort { get; set; } = NullSort.Undefined;

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Value.GetInternalQueries())
        {
            yield return item;
        }
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Value.GetPhysicalTables())
        {
            yield return item;
        }
    }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Value.GetCommonTables())
        {
            yield return item;
        }
    }

    public IEnumerable<QueryParameter> GetParameters()
    {
        return Value.GetParameters();
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        if (!IsAscending) yield return Token.Reserved(this, parent, "desc");
        if (NullSort != NullSort.Undefined) yield return Token.Reserved(this, parent, NullSort.ToCommandText());
    }
}