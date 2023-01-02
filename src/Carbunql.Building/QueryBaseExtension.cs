using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

public static class QueryBaseExtension
{
    public static SelectQuery ToCTE(this QueryBase source, string alias)
    {
        var sq = new SelectQuery();
        if (source.WithClause == null)
        {
            sq.WithClause = new();
        }
        else
        {
            sq.WithClause = new WithClause(source.WithClause);
            source.WithClause = null;
        }

        var ct = new CommonTable(new VirtualTable(source), alias);
        sq.WithClause.Add(ct);

        return sq;
    }

    public static (SelectQuery, FromClause) ToSubQuery(this QueryBase source, string alias)
    {
        var sq = new SelectQuery();
        if (source.WithClause == null)
        {
            sq.WithClause = new();
        }
        else
        {
            sq.WithClause = new WithClause(source.WithClause);
            source.WithClause = null;
        }

        var f = sq.From(source, alias);

        return (sq, f);
    }
}
