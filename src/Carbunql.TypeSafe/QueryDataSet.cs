using Carbunql.Analysis.Parser;
using Carbunql.Building;

namespace Carbunql.TypeSafe;

public class QueryDataSet(SelectQuery query) : ITypeSafeDataSet
{
    public SelectQuery Query { get; init; } = query;

    public SelectQuery BuildFromClause(SelectQuery query, string alias)
    {
        query.From(Query).As(alias);
        return query;
    }

    public SelectQuery BuildJoinClause(SelectQuery query, string join, string alias, string condition)
    {
        var r = query.FromClause!.Join(Query.ToSelectableTable(), join).As(alias);
        if (!string.IsNullOrEmpty(condition))
        {
            r.On(_ => ValueParser.Parse(condition));
        }
        return query;
    }
}
