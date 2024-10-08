﻿using Carbunql.Clauses;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for adding and clearing comment clauses for objects implementing the <see cref="ICommentable"/> interface.
/// </summary>
public static class ICommentableExtension
{
    /// <summary>
    /// Adds a comment to the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    /// <param name="comment">The comment to add.</param>
    [Obsolete]
    public static void AddComment(this ICommentable source, string comment)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Add(comment);
    }

    /// <summary>
    /// Clears any existing comments associated with the object.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="ICommentable"/> interface.</param>
    public static void ClearComment(this ICommentable source)
    {
        source.CommentClause ??= new CommentClause();
        source.CommentClause.Clear();
    }
}
