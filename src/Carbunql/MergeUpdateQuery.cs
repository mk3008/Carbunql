using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql;

/// <summary>
/// Represents a merge update query.
/// </summary>
public class MergeUpdateQuery : IQueryCommandable
{
    /// <summary>
    /// Gets or sets the set clause for the merge update query.
    /// </summary>
    public MergeSetClause? SetClause { get; set; }

    /// <summary>
    /// Retrieves the internal queries associated with the merge update query.
    /// </summary>
    /// <returns>The internal queries associated with the merge update query.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (SetClause != null)
        {
            foreach (var item in SetClause.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables associated with the merge update query.
    /// </summary>
    /// <returns>The physical tables associated with the merge update query.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (SetClause != null)
        {
            foreach (var item in SetClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables associated with the merge update query.
    /// </summary>
    /// <returns>The common tables associated with the merge update query.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (SetClause != null)
        {
            foreach (var item in SetClause.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the parameters of the merge update query.
    /// </summary>
    /// <returns>The parameters of the merge update query.</returns>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        if (SetClause != null)
        {
            foreach (var item in SetClause.GetParameters())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the tokens associated with the merge update query.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>The tokens associated with the merge update query.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (SetClause == null) throw new NullReferenceException();

        var t = Token.Reserved(this, parent, "update set");
        yield return t;
        foreach (var item in SetClause.GetTokens(t)) yield return item;
    }
}
