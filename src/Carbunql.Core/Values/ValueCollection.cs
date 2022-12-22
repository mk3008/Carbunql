using Carbunql.Core.Clauses;
using System.Collections;

namespace Carbunql.Core.Values;

public class ValueCollection : IList<ValueBase>, IQueryCommand
{
    public ValueCollection()
    {
    }

    public ValueCollection(string text)
    {
        Collection.Add(new LiteralValue(text));
    }

    public ValueCollection(ValueBase item)
    {
        Collection.Add(item);
    }

    public ValueCollection(List<ValueBase> collection)
    {
        Collection.AddRange(collection);
    }

    private List<ValueBase> Collection { get; init; } = new();

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var isFirst = true;
        foreach (var item in Collection)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, parent);
            }
            foreach (var token in item.GetTokens(parent)) yield return token;
        }
    }

    #region implements IList<ValueBase>
    public ValueBase this[int index]
    { get => ((IList<ValueBase>)Collection)[index]; set => ((IList<ValueBase>)Collection)[index] = value; }

    public int Count => ((ICollection<ValueBase>)Collection).Count;

    public bool IsReadOnly => ((ICollection<ValueBase>)Collection).IsReadOnly;

    public void Add(ValueBase item)
    {
        ((ICollection<ValueBase>)Collection).Add(item);
    }

    public void Clear()
    {
        ((ICollection<ValueBase>)Collection).Clear();
    }

    public bool Contains(ValueBase item)
    {
        return ((ICollection<ValueBase>)Collection).Contains(item);
    }

    public void CopyTo(ValueBase[] array, int arrayIndex)
    {
        ((ICollection<ValueBase>)Collection).CopyTo(array, arrayIndex);
    }

    public IEnumerator<ValueBase> GetEnumerator()
    {
        return ((IEnumerable<ValueBase>)Collection).GetEnumerator();
    }

    public int IndexOf(ValueBase item)
    {
        return ((IList<ValueBase>)Collection).IndexOf(item);
    }

    public void Insert(int index, ValueBase item)
    {
        ((IList<ValueBase>)Collection).Insert(index, item);
    }

    public bool Remove(ValueBase item)
    {
        return ((ICollection<ValueBase>)Collection).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<ValueBase>)Collection).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Collection).GetEnumerator();
    }
    #endregion
}