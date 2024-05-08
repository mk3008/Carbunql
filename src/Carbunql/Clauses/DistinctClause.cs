using Carbunql.Tables;
using MessagePack;
using System.Collections;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a DISTINCT clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class DistinctClause : IList<ValueBase>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DistinctClause"/> class.
    /// </summary>
    public DistinctClause()
    {
        Items = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DistinctClause"/> class with the specified items.
    /// </summary>
    public DistinctClause(IList<ValueBase> items)
    {
        Items = new();
        Items.AddRange(items);
    }

    /// <summary>
    /// Gets the items in the DISTINCT clause.
    /// </summary>
    private List<ValueBase> Items { get; init; }

    /// <summary>
    /// Gets the internal queries associated with this DISTINCT clause.
    /// </summary>
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

    /// <summary>
    /// Gets the physical tables associated with this DISTINCT clause.
    /// </summary>
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

    /// <summary>
    /// Gets the common tables associated with this DISTINCT clause.
    /// </summary>
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

    /// <summary>
    /// Gets the tokens representing this DISTINCT clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "distinct");
        yield return clause;

        if (!Items.Any()) yield break;

        yield return Token.Reserved(this, parent, "on");
        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;

        var isFirst = true;
        foreach (var item in Items)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, bracket);
            }
            foreach (var token in item.GetTokens(bracket)) yield return token;
        }

        if (!isFirst)
        {
            yield return Token.ReservedBracketEnd(this, parent);
        }
    }

    /// <summary>
    /// Gets the parameters associated with this DISTINCT clause.
    /// </summary>
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
    public int Count => ((ICollection<ValueBase>)Items).Count;

    public bool IsReadOnly => ((ICollection<ValueBase>)Items).IsReadOnly;

    public ValueBase this[int index] { get => ((IList<ValueBase>)Items)[index]; set => ((IList<ValueBase>)Items)[index] = value; }

    public int IndexOf(ValueBase item)
    {
        return ((IList<ValueBase>)Items).IndexOf(item);
    }

    public void Insert(int index, ValueBase item)
    {
        ((IList<ValueBase>)Items).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ((IList<ValueBase>)Items).RemoveAt(index);
    }

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

    public bool Remove(ValueBase item)
    {
        return ((ICollection<ValueBase>)Items).Remove(item);
    }

    public IEnumerator<ValueBase> GetEnumerator()
    {
        return ((IEnumerable<ValueBase>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }
    #endregion
}
