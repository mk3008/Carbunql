using System.Collections;

namespace Carbunql.Clauses;

public abstract class QueryCommandCollection<T> : IList<T> where T : IQueryCommand
{
    public QueryCommandCollection()
    {
    }

    public QueryCommandCollection(List<T> collection)
    {
        Items.AddRange(collection);
    }

    public virtual IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!Items.Any()) yield break;

        var isFirst = true;
        foreach (var item in Items)
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

    public List<T> Items { get; set; } = new();

    #region implements IList<T>
    public T this[int index] { get => ((IList<T>)Items)[index]; set => ((IList<T>)Items)[index] = value; }

    public int Count => ((ICollection<T>)Items).Count;

    public bool IsReadOnly => ((ICollection<T>)Items).IsReadOnly;

    public void Add(T item)
    {
        ((ICollection<T>)Items).Add(item);
    }

    public void Clear()
    {
        ((ICollection<T>)Items).Clear();
    }

    public bool Contains(T item)
    {
        return ((ICollection<T>)Items).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)Items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)Items).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return ((IList<T>)Items).IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        ((IList<T>)Items).Insert(index, item);
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)Items).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<T>)Items).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }

    #endregion
}