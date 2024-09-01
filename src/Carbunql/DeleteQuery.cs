using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents a DELETE query.
/// </summary>
public class DeleteQuery : IQueryCommandable, IReturning, ICommentable
{
    /// <summary>
    /// Gets or sets the delete clause.
    /// </summary>
    public DeleteClause? DeleteClause { get; set; }

    /// <summary>
    /// Gets or sets the with clause.
    /// </summary>
    public WithClause? WithClause { get; set; }

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
    /// Gets or sets the parameters.
    /// </summary>
    public IEnumerable<QueryParameter>? Parameters { get; set; }

    /// <summary>
    /// Gets the internal queries.
    /// </summary>
    /// <returns>Internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (DeleteClause != null)
        {
            foreach (var item in DeleteClause.GetInternalQueries())
            {
                yield return item;
            }
        }
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetInternalQueries())
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
    /// Gets the physical tables.
    /// </summary>
    /// <returns>Physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (DeleteClause != null)
        {
            foreach (var item in DeleteClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetPhysicalTables())
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
    /// Gets the parameters.
    /// </summary>
    /// <returns>Query parameters.</returns>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        if (Parameters != null)
        {
            foreach (var item in Parameters)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Gets the tokens.
    /// </summary>
    /// <param name="parent">Parent token.</param>
    /// <returns>Tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (DeleteClause == null) throw new NullReferenceException(nameof(DeleteClause));

        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        if (WithClause != null) foreach (var item in WithClause.GetTokens(parent)) yield return item;
        foreach (var item in DeleteClause.GetTokens(parent)) yield return item;
        if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;

        if (ReturningClause == null) yield break;
        foreach (var item in ReturningClause.GetTokens(parent)) yield return item;
    }

    /// <summary>
    /// Gets the common tables.
    /// </summary>
    /// <returns>Common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (DeleteClause != null)
        {
            foreach (var item in DeleteClause.GetCommonTables())
            {
                yield return item;
            }
        }
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetCommonTables())
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

    public IEnumerable<ColumnValue> GetColumns()
    {
        if (DeleteClause != null)
        {
            foreach (var item in DeleteClause.GetColumns())
            {
                yield return item;
            }
        }
        if (WithClause != null)
        {
            foreach (var item in WithClause.GetColumns())
            {
                yield return item;
            }
        }
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetColumns())
            {
                yield return item;
            }
        }
        if (ReturningClause != null)
        {
            foreach (var item in ReturningClause.GetColumns())
            {
                yield return item;
            }
        }
    }
}
