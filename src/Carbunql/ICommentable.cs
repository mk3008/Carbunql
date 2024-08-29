using Carbunql.Clauses;

namespace Carbunql;

/// <summary>
/// Represents an interface for objects that can have comment clauses.
/// </summary>
public interface ICommentable
{
    /// <summary>
    /// Gets or sets the comment clause associated with the object.
    /// </summary>
    CommentClause? CommentClause { get; set; }
}
