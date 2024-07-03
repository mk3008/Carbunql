using Carbunql.Tables;
using Carbunql.Values;
using System.Collections;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a collection of conditions for a WHEN clause in a MERGE statement.
/// </summary>
public class WhenClause : IList<MergeCondition>, IQueryCommandable
{
    /// <summary>
    /// Gets or sets the list of merge conditions.
    /// </summary>
    public List<MergeCondition> Conditions { get; set; } = new();

    /// <summary>
    /// Retrieves the internal queries associated with this when clause.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var condition in Conditions)
        {
            foreach (var item in condition.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with this when clause.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var condition in Conditions)
        {
            foreach (var item in condition.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with this when clause.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var condition in Conditions)
        {
            foreach (var item in condition.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the query parameters associated with this when clause.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Conditions)
        {
            foreach (var p in item.GetParameters())
            {
                yield return p;
            }
        }
    }

    /// <summary>
    /// Retrieves the tokens associated with this when clause.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var condition in Conditions)
        {
            foreach (var item in condition.GetTokens(parent))
            {
                yield return item;
            }
        }
    }

    #region implements IList<MergeCondition>
    public MergeCondition this[int index] { get => ((IList<MergeCondition>)Conditions)[index]; set => ((IList<MergeCondition>)Conditions)[index] = value; }

    public int Count => ((ICollection<MergeCondition>)Conditions).Count;

    public bool IsReadOnly => ((ICollection<MergeCondition>)Conditions).IsReadOnly;

    public void Add(MergeCondition item)
    {
        ((ICollection<MergeCondition>)Conditions).Add(item);
    }

    public void Clear()
    {
        ((ICollection<MergeCondition>)Conditions).Clear();
    }

    public bool Contains(MergeCondition item)
    {
        return ((ICollection<MergeCondition>)Conditions).Contains(item);
    }

    public void CopyTo(MergeCondition[] array, int arrayIndex)
    {
        ((ICollection<MergeCondition>)Conditions).CopyTo(array, arrayIndex);
    }

    public IEnumerator<MergeCondition> GetEnumerator()
    {
        return ((IEnumerable<MergeCondition>)Conditions).GetEnumerator();
    }

    public int IndexOf(MergeCondition item)
    {
        return ((IList<MergeCondition>)Conditions).IndexOf(item);
    }

    public void Insert(int index, MergeCondition item)
    {
        ((IList<MergeCondition>)Conditions).Insert(index, item);
    }

    public bool Remove(MergeCondition item)
    {
        return ((ICollection<MergeCondition>)Conditions).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<MergeCondition>)Conditions).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Conditions).GetEnumerator();
    }
    #endregion

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var condition in Conditions)
        {
            foreach (var item in condition.GetColumns())
            {
                yield return item;
            }
        }
    }
}