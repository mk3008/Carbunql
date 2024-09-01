using Carbunql.Clauses;

namespace Carbunql.Fluent;

/// <summary>
/// Provides extension methods for adding and clearing comment clauses for objects implementing the <see cref="ICommentable"/> interface.
/// </summary>
public static class ICommentableExtensions
{
    /// <summary>
    /// Adds a comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    public static CreateTableQuery Comment(this CreateTableQuery source, string comment)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Add(comment);
        return source;
    }

    /// <summary>
    /// Adds a comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    public static InsertQuery Comment(this InsertQuery source, string comment)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Add(comment);
        return source;
    }

    /// <summary>
    /// Adds a comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    public static UpdateQuery Comment(this UpdateQuery source, string comment)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Add(comment);
        return source;
    }

    /// <summary>
    /// Adds a comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    public static DeleteQuery Comment(this DeleteQuery source, string comment)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Add(comment);
        return source;
    }

    /// <summary>
    /// Adds a comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    public static MergeQuery Comment(this MergeQuery source, string comment)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Add(comment);
        return source;
    }

    /// <summary>
    /// Adds a header comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    public static SelectQuery HeaderComment(this SelectQuery source, string comment)
    {
        source.HeaderCommentClause ??= new CommentClause();
        source.HeaderCommentClause.Add(comment);
        return source;
    }

    /// <summary>
    /// Adds a comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    public static SelectQuery Comment(this SelectQuery source, string comment)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Add(comment);
        return source;
    }

    /// <summary>
    /// Clears any existing comments associated with the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    public static SelectQuery ClearComment(this SelectQuery source)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Clear();
        return source;
    }
}
