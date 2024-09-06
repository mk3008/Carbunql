using Carbunql.Tables;
using Carbunql.Values;
using System.Collections;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a GROUP BY clause in a SQL query.
/// </summary>
public class GroupClause : IList<ValueBase>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupClause"/> class.
    /// </summary>
    public GroupClause()
    {
        Items = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupClause"/> class with the specified list of items.
    /// </summary>
    public GroupClause(IList<ValueBase> items)
    {
        Items = new();
        Items.AddRange(items);
    }

    private List<ValueBase> Items { get; init; }

    /// <summary>
    /// Gets the internal queries associated with this GROUP BY clause.
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
    /// Gets the physical tables associated with this GROUP BY clause.
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
    /// Gets the common tables associated with this GROUP BY clause.
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
    /// Gets the tokens representing this GROUP BY clause.
    /// </summary>
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

    /// <summary>
    /// Gets the parameters associated with this GROUP BY clause.
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

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetColumns())
            {
                yield return item;
            }
        }
    }
}