using Carbunql.Analysis.Parser;
using Carbunql.Building;

namespace Carbunql.TypeSafe;

public class CTEDataSet(string name, SelectQuery query) : IDataSet
{
    public string Name { get; set; } = name;

    public Materialized Materialized { get; set; } = Materialized.Undefined;

    public SelectQuery Query { get; init; } = query;

    public List<string> Columns { get; init; } = query.GetColumnNames().ToList();

    public SelectQuery BuildFromClause(SelectQuery query, string alias)
    {
        var cte = query.With(Query).As(Name);
        cte.Materialized = Materialized;
        query.From(new CTETable(cte).ToSelectable()).As(alias);
        return query;
    }

    public SelectQuery BuildJoinClause(SelectQuery query, string join, string alias, string condition)
    {
        var cte = query.With(Query).As(Name);
        cte.Materialized = Materialized;

        var r = query.FromClause!.Join(cte, join).As(alias);
        if (!string.IsNullOrEmpty(condition))
        {
            r.On(_ => ValueParser.Parse(condition));
        }
        return query;
    }
}
