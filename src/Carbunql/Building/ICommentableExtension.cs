using Carbunql.Clauses;

namespace Carbunql.Building;

public interface ICommentable
{
	CommentClause? CommentClause { get; set; }
}

public static class ICommentableExtension
{
	public static void AddComment(this ICommentable source, string comment)
	{
		source.CommentClause ??= new();
		source.CommentClause.Add(comment);
	}

	public static void ClearComment(this ICommentable source)
	{
		source.CommentClause ??= new();
		source.CommentClause.Clear();
	}
}
