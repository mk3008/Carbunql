using Carbunql.Clauses;

namespace Carbunql.Building;

public static class QueryBaseExtension
{
    public static CTEQuery ToCTE(this CTEQuery source, string alias)
    {
        var sq = new CTEQuery();

        foreach (var item in source.WithClause.CommonTables)
        {
            sq.WithClause.Add(item);
        }

        if (source.Query != null)
        {
            sq.WithClause.Add(source.Query.ToCommonTable(alias));
        }

        return sq;
    }

    public static (CTEQuery, FromClause) ToSubQuery(this CTEQuery source, string alias)
    {
        if (source.Query == null) throw new NullReferenceException(nameof(source.Query));

        var sq = new SelectQuery();
        var f = sq.From(source.Query, alias);

        var cteq = new CTEQuery();
        foreach (var item in source.WithClause.CommonTables)
        {
            cteq.WithClause.Add(item);
        }
        cteq.Query = sq;
        return (cteq, f);
    }
}