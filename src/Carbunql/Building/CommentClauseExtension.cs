namespace Carbunql.Building;

public static class CommentClauseExtension
{
	public static void AddComment(this SelectQuery source, string comment)
	{
		source.CommentClause ??= new();
		source.CommentClause.Add(comment);
	}
}
