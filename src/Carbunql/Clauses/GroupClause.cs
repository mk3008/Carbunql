using Carbunql.Tables;
using MessagePack;
using System.Collections;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class GroupClause : IList<ValueBase>, IQueryCommandable
{
    public GroupClause()
    {
        Items = new();
    }

    public GroupClause(IList<ValueBase> items)
    {
        Items = new();
        Items.AddRange(items);
    }

    private List<ValueBase> Items { get; init; }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "group by");
        yield return clause;

        var isFirst = true;
        foreach (var item in Items)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, clause);
            }
            foreach (var token in item.GetTokens(clause)) yield return token;
        }
    }

    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Items)
        {
            foreach (var p in item.GetParameters())
            {
                yield return p;
            }
        }
    }

    #region implements IList<ValueBase>
    public ValueBase this[int index] { get => ((IList<ValueBase>)Items)[index]; set => ((IList<ValueBase>)Items)[index] = value; }

    public int Count => ((ICollection<ValueBase>)Items).Count;

    public bool IsReadOnly => ((ICollection<ValueBase>)Items).IsReadOnly;

    public void Add(ValueBase item)
    {
        ((ICollection<ValueBase>)Items).Add(item);
    }

    public void Clear()
    {
        ((ICollection<ValueBase>)Items).Clear();
    }

    public bool Contains(ValueBase item)
    {
        return ((ICollection<ValueBase>)Items).Contains(item);
    }

    public void CopyTo(ValueBase[] array, int arrayIndex)
    {
        ((ICollection<ValueBase>)Items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<ValueBase> GetEnumerator()
    {
        return ((IEnumerable<ValueBase>)Items).GetEnumerator();
    }

    public int IndexOf(ValueBase item)
    {
        return ((IList<ValueBase>)Items).IndexOf(item);
    }

    public void Insert(int index, ValueBase item)
    {
        ((IList<ValueBase>)Items).Insert(index, item);
    }

    public bool Remove(ValueBase item)
    {
        return ((ICollection<ValueBase>)Items).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<ValueBase>)Items).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }
    #endregion
}