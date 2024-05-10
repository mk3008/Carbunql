using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents an update query.
/// </summary>
public class UpdateQuery : IQueryCommandable, IReturning, ICommentable
{
    /// <summary>
    /// Gets or sets the update clause.
    /// </summary>
    public UpdateClause? UpdateClause { get; set; }

    /// <summary>
    /// Gets or sets the with clause.
    /// </summary>
    public WithClause? WithClause { get; set; }

    /// <summary>
    /// Gets or sets the set clause.
    /// </summary>
    public SetClause? SetClause { get; set; }

    /// <summary>
    /// Gets or sets the from clause.
    /// </summary>
    public FromClause? FromClause { get; set; }

    /// <summary>
    /// Gets or sets the where clause.
    /// </summary>
    public WhereClause? WhereClause { get; set; }

    /// <summary>
    /// Gets or sets the returning clause.
    /// </summary>
    public ReturningClause? ReturningClause { get; set; }

    /// <summary>
    /// Gets or sets the comment clause.
    /// </summary>
    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    /// <summary>
    /// Retrieves internal queries contained within the update query.
    /// </summary>
    /// <returns>An enumerable collection of internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (UpdateClause != null)
        {
            foreach (var item in UpdateClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (SetClause != null)
        {
            foreach (var value in SetClause)
            {
                foreach (var item in value.GetInternalQueries())
                {
                    yield return item;
                }
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (ReturningClause != null)
        {
            foreach (var item in ReturningClause.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the physical tables involved in the update query.
    /// </summary>
    /// <returns>An enumerable collection of physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (UpdateClause != null)
        {
            foreach (var item in UpdateClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (SetClause != null)
        {
            foreach (var value in SetClause)
            {
                foreach (var item in value.GetPhysicalTables())
                {
                    yield return item;
                }
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (ReturningClause != null)
        {
            foreach (var item in ReturningClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the common tables involved in the update query.
    /// </summary>
    /// <returns>An enumerable collection of common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (UpdateClause != null)
        {
            foreach (var item in UpdateClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (SetClause != null)
        {
            foreach (var value in SetClause)
            {
                foreach (var item in value.GetCommonTables())
                {
                    yield return item;
                }
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (ReturningClause != null)
        {
            foreach (var item in ReturningClause.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    public IEnumerable<QueryParameter>? Parameters { get; set; }

    /// <summary>
    /// Retrieves the parameters used in the update query.
    /// </summary>
    /// <returns>An enumerable collection of query parameters.</returns>
    public IEnumerable<QueryParameter> GetParameters()
    {
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetParameters())
            {
                yield return item;
            }
        }
        if (SetClause != null)
        {
            foreach (var item in SetClause.GetParameters())
            {
                yield return item;
            }
        }
        if (FromClause != null)
        {
            foreach (var item in FromClause.GetParameters())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetParameters())
            {
                yield return item;
            }
        }
        if (ReturningClause != null)
        {
            foreach (var item in ReturningClause.GetParameters())
            {
                yield return item;
            }
        }
        if (Parameters != null)
        {
            foreach (var item in Parameters)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the tokens representing the update query.
    /// </summary>
    /// <param name="parent">The parent token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (UpdateClause == null) throw new NullReferenceException(nameof(UpdateClause));
        if (SetClause == null) throw new NullReferenceException(nameof(SetClause));

        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        if (parent == null && WithClause != null)
        {
            var lst = GetCommonTables().ToList();
            foreach (var item in WithClause.GetTokens(parent, lst)) yield return item;
        }

        foreach (var item in UpdateClause.GetTokens(parent)) yield return item;
        foreach (var item in SetClause.GetTokens(parent)) yield return item;
        if (FromClause != null) foreach (var item in FromClause.GetTokens(parent)) yield return item;
        if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;

        if (ReturningClause == null) yield break;
        foreach (var item in ReturningClause.GetTokens(parent)) yield return item;
    }
}
