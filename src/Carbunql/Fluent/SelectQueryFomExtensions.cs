using Carbunql.Clauses;

namespace Carbunql.Fluent;

public static class SelectQueryFomExtensions
{
    public static SelectQuery From(this SelectQuery query, FluentTable table)
    {
        if (query.FromClause != null) throw new InvalidOperationException("FromClause is already exists.");
        query.FromClause = new FromClause(table.ToSelectable());

        if (table.IsCommonTable)
        {
            query.With(table);
        }

        return query;
    }

    public static SelectQuery From(this SelectQuery query, string table, string alias)
    {
        query.AddFrom(table, alias);
        return query;
    }
}
