using Carbunql.Tables;
using MessagePack;
using System.Collections;

namespace Carbunql.Clauses;

// <summary>
/// Represents a WITH clause in a query, containing a list of common tables.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class WithClause : IList<CommonTable>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WithClause"/> class.
    /// </summary>
    public WithClause()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WithClause"/> class with the specified common tables.
    /// </summary>
    /// <param name="commons">The list of common tables.</param>
    public WithClause(IList<CommonTable> commons)
    {
        CommonTables.AddRange(commons);
    }

    /// <summary>
    /// Gets or sets the list of common tables in the WITH clause.
    /// </summary>
    public List<CommonTable> CommonTables { get; private set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the WITH clause contains the 'RECURSIVE' keyword.
    /// </summary>
    public bool HasRecursiveKeyword { get; set; } = false;

    public IEnumerable<Token> GetTokens(Token? parent, IEnumerable<CommonTable> commons)
    {
        if (parent != null) yield break;

        Token? clause = null;

        var dic = new Dictionary<string, CommonTable>();
        foreach (var item in commons)
        {
            if (dic.ContainsKey(item.Alias)) continue;
            dic.Add(item.Alias, item);

            if (clause == null)
            {
                if (HasRecursiveKeyword)
                {
                    clause = Token.Reserved(this, null, "with recursive");
                }
                else
                {
                    clause = Token.Reserved(this, null, "with");
                }
                yield return clause;
            }
            else
            {
                yield return Token.Comma(this, clause);
            }
            foreach (var token in item.GetTokens(clause!)) yield return token;
        }
    }

    /// <summary>
    /// Retrieves the tokens associated with the WITH clause.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in GetTokens(parent, CommonTables))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves the query parameters associated with the WITH clause.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var ct in CommonTables)
        {
            foreach (var prm in ct.GetParameters())
            {
                yield return prm;
            }
        }
    }

    /// <summary>
    /// Retrieves the internal queries associated with the WITH clause.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var commonTable in CommonTables)
        {
            foreach (var item in commonTable.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with the WITH clause.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var commonTable in CommonTables)
        {
            foreach (var item in commonTable.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with the WITH clause.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var commonTable in CommonTables)
        {
            foreach (var item in commonTable.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    #region implements IList<CommonTable>
    public CommonTable this[int index] { get => ((IList<CommonTable>)CommonTables)[index]; set => ((IList<CommonTable>)CommonTables)[index] = value; }

    public int Count => ((ICollection<CommonTable>)CommonTables).Count;

    public bool IsReadOnly => ((ICollection<CommonTable>)CommonTables).IsReadOnly;

    public void Add(CommonTable item)
    {
        ((ICollection<CommonTable>)CommonTables).Add(item);
    }

    public void Clear()
    {
        ((ICollection<CommonTable>)CommonTables).Clear();
    }

    public bool Contains(CommonTable item)
    {
        return ((ICollection<CommonTable>)CommonTables).Contains(item);
    }

    public void CopyTo(CommonTable[] array, int arrayIndex)
    {
        ((ICollection<CommonTable>)CommonTables).CopyTo(array, arrayIndex);
    }

    public IEnumerator<CommonTable> GetEnumerator()
    {
        return ((IEnumerable<CommonTable>)CommonTables).GetEnumerator();
    }

    public int IndexOf(CommonTable item)
    {
        return ((IList<CommonTable>)CommonTables).IndexOf(item);
    }

    public void Insert(int index, CommonTable item)
    {
        ((IList<CommonTable>)CommonTables).Insert(index, item);
    }

    public bool Remove(CommonTable item)
    {
        return ((ICollection<CommonTable>)CommonTables).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<CommonTable>)CommonTables).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)CommonTables).GetEnumerator();
    }
    #endregion
}