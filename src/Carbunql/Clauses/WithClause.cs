using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;
using System.Collections;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a WITH clause in a query, which contains a list of common tables.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class WithClause : IList<CommonTable>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WithClause"/> class without any common tables defined.
    /// Common tables can be added later using the AddCommonTable method.
    /// </summary>
    public WithClause()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WithClause"/> class with the specified list of common tables.
    /// </summary>
    /// <param name="commons">The list of common tables to include in the WITH clause.</param>
    /// <remarks>
    /// The WITH clause, also known as Common Table Expression (CTE), allows you to define temporary named result sets that can be referenced within the scope of the main query.
    /// This constructor initializes the WITH clause with the provided list of common tables.
    /// </remarks>
    public WithClause(IList<CommonTable> commons)
    {
        CommonTables.AddRange(commons);
    }

    /// <summary>
    /// Gets or sets the list of common tables included in the WITH clause.
    /// </summary>
    /// <remarks>
    /// The WITH clause, also known as Common Table Expression (CTE), allows you to define temporary named result sets that can be referenced within the scope of the main query.
    /// This property represents the list of common tables to be included in the WITH clause.
    /// </remarks>
    private List<CommonTable> CommonTables { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the WITH clause contains the 'RECURSIVE' keyword.
    /// </summary>
    /// <remarks>
    /// The 'RECURSIVE' keyword is used in a Common Table Expression (CTE) within a WITH clause to define a recursive CTE.
    /// It specifies that the CTE can reference itself in its definition.
    /// </remarks>
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

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
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