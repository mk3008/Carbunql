using Carbunql.Clauses;
using Carbunql.Tables;

namespace Carbunql.Building;

public static class ReadQueryExtension
{
    public static CTEQuery ToCTE(this IReadQuery source, string alias)
    {
        var sq = new CTEQuery();

        sq.WithClause.Add(source.ToCommonTable(alias));

        return sq;
    }

    public static CommonTable ToCommonTable(this IReadQuery source, string alias)
    {
        return new CommonTable(new VirtualTable(source), alias);
    }

    public static (ReadQuery, FromClause) ToSubQuery(this IReadQuery source, string alias)
    {
        var sq = new SelectQuery();
        var f = sq.From(source, alias);
        return (sq, f);
    }
}