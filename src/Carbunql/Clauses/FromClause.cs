using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a FROM clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class FromClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FromClause"/> class with the specified root selectable table.
    /// </summary>
    public FromClause(SelectableTable root)
    {
        Root = root;
    }

    /// <summary>
    /// Gets or sets the root selectable table in the FROM clause.
    /// </summary>
    public SelectableTable Root { get; init; }

    /// <summary>
    /// Gets or sets the list of relations in the FROM clause.
    /// </summary>
    public List<Relation>? Relations { get; set; }

    /// <summary>
    /// Gets the internal queries associated with this FROM clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Root.GetInternalQueries())
        {
            yield return item;
        }

        if (Relations != null)
        {
            foreach (var relation in Relations)
            {
                foreach (var item in relation.GetInternalQueries())
                {
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// Gets the physical tables associated with this FROM clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Root.GetPhysicalTables())
        {
            yield return item;
        }

        if (Relations != null)
        {
            foreach (var relation in Relations)
            {
                foreach (var item in relation.GetPhysicalTables())
                {
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// Gets the selectable tables associated with this FROM clause.
    /// </summary>
    public IEnumerable<SelectableTable> GetSelectableTables()
    {
        yield return Root;

        if (Relations != null)
        {
            foreach (var item in Relations)
            {
                yield return item.Table;
            }
        }
    }

    /// <summary>
    /// Gets the common tables associated with this FROM clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Root.GetCommonTables())
        {
            yield return item;
        }

        if (Relations != null)
        {
            foreach (var relation in Relations)
            {
                foreach (var item in relation.GetCommonTables())
                {
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// Gets the parameters associated with this FROM clause.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Root.GetParameters())
        {
            yield return item;
        }
        if (Relations != null)
        {
            foreach (var relation in Relations)
            {
                foreach (var item in relation.GetParameters())
                {
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// Gets the tokens representing this FROM clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "from");

        yield return clause;
        foreach (var item in Root.GetTokens(clause)) yield return item;

        if (Relations == null) yield break;

        foreach (var item in Relations)
        {
            foreach (var token in item.GetTokens(clause)) yield return token;
        }
    }
}
