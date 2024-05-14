using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a window definition used in a WINDOW clause of a query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class WindowDefinition : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowDefinition"/> class.
    /// </summary>
    public WindowDefinition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowDefinition"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the window.</param>
    public WindowDefinition(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowDefinition"/> class with the specified partition clause.
    /// </summary>
    /// <param name="partitionby">The partition clause.</param>
    public WindowDefinition(PartitionClause partitionby)
    {
        PartitionBy = partitionby;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowDefinition"/> class with the specified order clause.
    /// </summary>
    /// <param name="orderBy">The order clause.</param>
    public WindowDefinition(OrderClause orderBy)
    {
        OrderBy = orderBy;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowDefinition"/> class with the specified partition and order clauses.
    /// </summary>
    /// <param name="partitionby">The partition clause.</param>
    /// <param name="orderBy">The order clause.</param>
    public WindowDefinition(PartitionClause partitionby, OrderClause orderBy)
    {
        PartitionBy = partitionby;
        OrderBy = orderBy;
    }

    /// <summary>
    /// Gets or sets the name of the window.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the partition clause of the window.
    /// </summary>
    public PartitionClause? PartitionBy { get; set; }

    /// <summary>
    /// Gets or sets the order clause of the window.
    /// </summary>
    public OrderClause? OrderBy { get; set; }

    /// <summary>
    /// Retrieves the internal queries associated with this window definition.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (!string.IsNullOrEmpty(Name)) yield break;

        if (PartitionBy != null)
        {
            foreach (var item in PartitionBy.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (OrderBy != null)
        {
            foreach (var item in OrderBy.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the query parameters associated with this window definition.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    public IEnumerable<QueryParameter> GetParameters()
    {
        if (!string.IsNullOrEmpty(Name)) yield break;
        if (PartitionBy == null && OrderBy == null) yield break;

        if (PartitionBy != null)
        {
            foreach (var item in PartitionBy.GetParameters())
            {
                yield return item;
            }
        }
        if (OrderBy != null)
        {
            foreach (var item in OrderBy.GetParameters())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with this window definition.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (!string.IsNullOrEmpty(Name)) yield break;
        if (PartitionBy == null && OrderBy == null) yield break;

        if (PartitionBy != null)
        {
            foreach (var item in PartitionBy.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (OrderBy != null)
        {
            foreach (var item in OrderBy.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with this window definition.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (!string.IsNullOrEmpty(Name)) yield break;
        if (PartitionBy == null && OrderBy == null) yield break;

        if (PartitionBy != null)
        {
            foreach (var item in PartitionBy.GetCommonTables())
            {
                yield return item;
            }
        }
        if (OrderBy != null)
        {
            foreach (var item in OrderBy.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the tokens associated with this window definition.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            yield return new Token(this, parent, Name);
            yield break;
        }

        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;

        if (PartitionBy != null)
        {
            foreach (var item in PartitionBy.GetTokens(bracket))
            {
                yield return item;
            }
        }
        if (OrderBy != null)
        {
            foreach (var item in OrderBy.GetTokens(bracket))
            {
                yield return item;
            }
        }

        yield return Token.ReservedBracketEnd(this, parent);
    }
}
