using Carbunql.Clauses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Values;

public class SortableValueCollection : IList<SortableItem>
{
    public SortableValueCollection()
    {
    }

    public SortableValueCollection(List<SortableItem> collection)
    {
        Collection.AddRange(collection);
    }

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

    public SortableItem this[int index] { get => ((IList<SortableItem>)Collection)[index]; set => ((IList<SortableItem>)Collection)[index] = value; }

    public int Count => ((ICollection<SortableItem>)Collection).Count;

    public bool IsReadOnly => ((ICollection<SortableItem>)Collection).IsReadOnly;

    protected List<SortableItem> Collection { get; init; } = new();

    public void Add(SortableItem item)
    {
        ((ICollection<SortableItem>)Collection).Add(item);
    }

    public void Clear()
    {
        ((ICollection<SortableItem>)Collection).Clear();
    }

    public bool Contains(SortableItem item)
    {
        return ((ICollection<SortableItem>)Collection).Contains(item);
    }

    public void CopyTo(SortableItem[] array, int arrayIndex)
    {
        ((ICollection<SortableItem>)Collection).CopyTo(array, arrayIndex);
    }

    public IEnumerator<SortableItem> GetEnumerator()
    {
        return ((IEnumerable<SortableItem>)Collection).GetEnumerator();
    }

    public int IndexOf(SortableItem item)
    {
        return ((IList<SortableItem>)Collection).IndexOf(item);
    }

    public void Insert(int index, SortableItem item)
    {
        ((IList<SortableItem>)Collection).Insert(index, item);
    }

    public bool Remove(SortableItem item)
    {
        return ((ICollection<SortableItem>)Collection).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<SortableItem>)Collection).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Collection).GetEnumerator();
    }
}
