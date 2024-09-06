using Carbunql.Tables;
using Carbunql.Values;
using System.Collections;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a WINDOW clause in a SQL query.
/// </summary>
/// <remarks>
/// The WINDOW clause defines window specifications for window functions used in the query.
/// It allows specifying named windows, which can be referenced in window function calls.
/// </remarks>

public class WindowClause : IList<NamedWindowDefinition>, IQueryCommandable

{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowClause"/> class without any window definitions.
    /// </summary>
    public WindowClause()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowClause"/> class with the specified window definitions.
    /// </summary>
    /// <param name="definitions">The window definitions to be added to the window clause.</param>
    public WindowClause(IList<NamedWindowDefinition> definitions)
    {
        NamedWindowDefinitions.AddRange(definitions);
    }

    /// <summary>
    /// Gets the list of named window definitions associated with this WINDOW clause.
    /// </summary>
    public List<NamedWindowDefinition> NamedWindowDefinitions { get; private set; } = new();

    /// <summary>
    /// Retrieves the internal queries associated with this WINDOW clause.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var definition in NamedWindowDefinitions)
        {
            foreach (var item in definition.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the query parameters associated with this WINDOW clause.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in NamedWindowDefinitions)
        {
            foreach (var p in item.GetParameters())
            {
                yield return p;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with this WINDOW clause.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var definition in NamedWindowDefinitions)
        {
            foreach (var item in definition.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with this WINDOW clause.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var definition in NamedWindowDefinitions)
        {
            foreach (var item in definition.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the tokens associated with this WINDOW clause.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!NamedWindowDefinitions.Any()) yield break;

        var clause = Token.Reserved(this, parent, "window");
        yield return clause;

        var isFirst = true;
        foreach (var item in NamedWindowDefinitions)
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

    #region implements IList<NamedWindowDefinition>

    public NamedWindowDefinition this[int index] { get => ((IList<NamedWindowDefinition>)NamedWindowDefinitions)[index]; set => ((IList<NamedWindowDefinition>)NamedWindowDefinitions)[index] = value; }

    public int Count => ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Count;

    public bool IsReadOnly => ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).IsReadOnly;

    public void Add(NamedWindowDefinition item)
    {
        ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Add(item);
    }

    public void Clear()
    {
        ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Clear();
    }

    public bool Contains(NamedWindowDefinition item)
    {
        return ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Contains(item);
    }

    public void CopyTo(NamedWindowDefinition[] array, int arrayIndex)
    {
        ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).CopyTo(array, arrayIndex);
    }

    public IEnumerator<NamedWindowDefinition> GetEnumerator()
    {
        return ((IEnumerable<NamedWindowDefinition>)NamedWindowDefinitions).GetEnumerator();
    }

    public int IndexOf(NamedWindowDefinition item)
    {
        return ((IList<NamedWindowDefinition>)NamedWindowDefinitions).IndexOf(item);
    }

    public void Insert(int index, NamedWindowDefinition item)
    {
        ((IList<NamedWindowDefinition>)NamedWindowDefinitions).Insert(index, item);
    }

    public bool Remove(NamedWindowDefinition item)
    {
        return ((ICollection<NamedWindowDefinition>)NamedWindowDefinitions).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<NamedWindowDefinition>)NamedWindowDefinitions).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)NamedWindowDefinitions).GetEnumerator();
    }
    #endregion

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var definition in NamedWindowDefinitions)
        {
            foreach (var item in definition.GetColumns())
            {
                yield return item;
            }
        }
    }
}
