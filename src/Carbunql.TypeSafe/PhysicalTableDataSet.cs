using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;

namespace Carbunql.TypeSafe;

public class PhysicalTableDataSet(ITable tb, IEnumerable<string> columns) : ITypeSafeDataSet
{
    public SelectableTable Table { get; set; } = new PhysicalTable(tb).ToSelectable();

    public SelectQuery BuildFromClause(SelectQuery query, string alias)
    {
        query.From(Table).As(alias);
        return query;
    }

    public SelectQuery BuildJoinClause(SelectQuery query, string join, string alias, string condition)
    {
        var r = query.FromClause!.Join(Table, join).As(alias);
        if (!string.IsNullOrEmpty(condition))
        {
            r.On(_ => ValueParser.Parse(condition));
        }
        return query;
    }
}
