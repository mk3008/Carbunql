using Carbunql.Clauses;

namespace Carbunql;

public class SelectQuery : ReadQuery, IQueryCommandable
{
    public SelectClause? SelectClause { get; set; }

    public FromClause? FromClause { get; set; }

    public WhereClause? WhereClause { get; set; }

    public GroupClause? GroupClause { get; set; }

    public HavingClause? HavingClause { get; set; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (SelectClause == null) yield break;

        foreach (var item in SelectClause.GetTokens(parent)) yield return item;
        if (FromClause != null) foreach (var item in FromClause.GetTokens(parent)) yield return item;
        if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;
        if (GroupClause != null) foreach (var item in GroupClause.GetTokens(parent)) yield return item;
        if (HavingClause != null) foreach (var item in HavingClause.GetTokens(parent)) yield return item;
    }
}