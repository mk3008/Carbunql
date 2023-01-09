using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

public static class QueryBaseExtension
{
    public static SelectQuery GetSelectQuery(this QueryBase source)
    {
        return (SelectQuery)source.QueryWithoutCTE;
    }

    public static ValuesQuery GetValuesQuery(this QueryBase source)
    {
        return (ValuesQuery)source.QueryWithoutCTE;

    }

    public static CTEQuery ToCTE(this QueryBase source, string alias)
    {
        var sq = new CTEQuery();

        if (source.WithClause != null)
        {
            foreach (var item in source.WithClause.CommonTables)
            {
                sq.WithClause.Add(item);
            }
        }

        sq.WithClause.Add(source.ToCommonTable(alias));

        return sq;
    }

    public static CommonTable ToCommonTable(this QueryBase source, string alias)
    {
        return new CommonTable(new VirtualTable(source.QueryWithoutCTE), alias);
    }

    public static (QueryBase, FromClause) ToSubQuery(this QueryBase source, string alias)
    {
        var sq = new SelectQuery();
        var f = sq.From(source.QueryWithoutCTE, alias);

        if (source.WithClause == null) return (sq, f);

        var cteq = new CTEQuery();
        foreach (var item in source.WithClause.CommonTables)
        {
            cteq.WithClause.Add(item);
        }
        cteq.Query = sq;
        return (cteq, f);
    }
}
