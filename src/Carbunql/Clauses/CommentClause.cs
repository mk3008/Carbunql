using Carbunql.Tables;
using Carbunql.Values;
using System.Collections;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a collection of SQL comments.
/// </summary>
public class CommentClause : IList<string>, IQueryCommandable
{
    /// <summary>
    /// Gets or sets the comment at the specified index.
    /// </summary>
    public string this[int index] { get => ((IList<string>)Collection)[index]; set => ((IList<string>)Collection)[index] = value; }

    /// <summary>
    /// Gets the number of comments in the collection.
    /// </summary>
    public int Count => ((ICollection<string>)Collection).Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly => ((ICollection<string>)Collection).IsReadOnly;

    private List<string> Collection { get; set; } = new();

    /// <summary>
    /// Adds a comment to the collection.
    /// </summary>
    public void Add(string item)
    {
        ((ICollection<string>)Collection).Add(item);
    }

    /// <summary>
    /// Removes all comments from the collection.
    /// </summary>
    public void Clear()
    {
        ((ICollection<string>)Collection).Clear();
    }

    /// <summary>
    /// Determines whether the collection contains a specific comment.
    /// </summary>
    public bool Contains(string item)
    {
        return ((ICollection<string>)Collection).Contains(item);
    }

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    public void CopyTo(string[] array, int arrayIndex)
    {
        ((ICollection<string>)Collection).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    public IEnumerator<string> GetEnumerator()
    {
        return ((IEnumerable<string>)Collection).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Collection).GetEnumerator();
    }

    /// <summary>
    /// Gets the common tables associated with this clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the internal queries associated with this clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with this clause.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with this clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens representing this clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!Collection.Any()) yield break;

        foreach (var item in Collection)
        {
            yield return Token.Reserved(this, parent, "/*");
            yield return new Token(this, parent, item.Replace("/*", "").Replace("*/", ""));
            yield return Token.Reserved(this, parent, "*/");
        }
    }

    /// <summary>
    /// Searches for the specified comment and returns the zero-based index of the first occurrence within the entire collection.
    /// </summary>
    public int IndexOf(string item)
    {
        return ((IList<string>)Collection).IndexOf(item);
    }

    /// <summary>
    /// Inserts a comment into the collection at the specified index.
    /// </summary>
    public void Insert(int index, string item)
    {
        ((IList<string>)Collection).Insert(index, item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific comment from the collection.
    /// </summary>
    public bool Remove(string item)
    {
        return ((ICollection<string>)Collection).Remove(item);
    }

    /// <summary>
    /// Removes the comment at the specified index.
    /// </summary>
    public void RemoveAt(int index)
    {
        ((IList<string>)Collection).RemoveAt(index);
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}

