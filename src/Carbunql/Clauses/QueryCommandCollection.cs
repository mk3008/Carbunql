using MessagePack;
using System.Collections;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a collection of query commandable items.
/// </summary>
/// <typeparam name="T">The type of items in the collection, which must implement <see cref="IQueryCommandable"/>.</typeparam>
[MessagePackObject(keyAsPropertyName: true)]
public abstract class QueryCommandCollection<T> : IList<T> where T : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCommandCollection{T}"/> class.
    /// </summary>
    public QueryCommandCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCommandCollection{T}"/> class with the specified collection of items.
    /// </summary>
    /// <param name="collection">The collection of items.</param>
    public QueryCommandCollection(List<T> collection)
    {
        Items.AddRange(collection);
    }

    /// <summary>
    /// Gets or sets the list of items in the collection.
    /// </summary>
    protected List<T> Items { get; set; } = new();

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Items)
        {
            foreach (var prm in item.GetParameters())
            {
                yield return prm;
            }
        }
    }

    #region implements IList<T>
    [IgnoreMember]
    public T this[int index] { get => ((IList<T>)Items)[index]; set => ((IList<T>)Items)[index] = value; }

    [IgnoreMember]
    public int Count => ((ICollection<T>)Items).Count;

    [IgnoreMember]
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