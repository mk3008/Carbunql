namespace Carbunql.Fluent;

public static class SelectQueryJoinExtensions
{
    public static SelectQuery InnerJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        if (right.IsCommonTable)
        {
            query.With(right);
        }

        var condition = string.Join(" and ", columns.Select(x => $"{left.Alias}.{x} = {right.Alias}.{x}"));
        query.AddJoin("inner join", right.ToSelectable(), condition);
        return query;
    }

    public static SelectQuery LeftJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        if (right.IsCommonTable)
        {
            query.With(right);
        }

        var condition = string.Join(" and ", columns.Select(x => $"{left.Alias}.{x} = {right.Alias}.{x}"));
        query.AddJoin("left join", right.ToSelectable(), condition);
        return query;
    }

    public static SelectQuery RightJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        if (right.IsCommonTable)
        {
            query.With(right);
        }

        var condition = string.Join(" and ", columns.Select(x => $"{left.Alias}.{x} = {right.Alias}.{x}"));
        query.AddJoin("right join", right.ToSelectable(), condition);
        return query;
    }

    public static SelectQuery CrossJoin(this SelectQuery query, FluentTable right)
    {
        if (right.IsCommonTable)
        {
            query.With(right);
        }

        query.AddJoin("cross join", right.ToSelectable());
        return query;
    }

    public static SelectQuery InnerJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        var condition = string.Join(" and ", columns.Select(x => $"{leftTableAlias}.{x} = {rightTableAlias}.{x}"));
        query.AddJoin("inner join", rightTableName, rightTableAlias, condition);
        return query;
    }

    public static SelectQuery LeftJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        var condition = string.Join(" and ", columns.Select(x => $"{leftTableAlias}.{x} = {rightTableAlias}.{x}"));
        query.AddJoin("left join", rightTableName, rightTableAlias, condition);
        return query;
    }

    public static SelectQuery RightJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        var condition = string.Join(" and ", columns.Select(x => $"{leftTableAlias}.{x} = {rightTableAlias}.{x}"));
        query.AddJoin("right join", rightTableName, rightTableAlias, condition);
        return query;
    }

    public static SelectQuery CrossJoin(this SelectQuery query, string rightTableName, string rightTableAlias)
    {
        query.AddJoin("cross join", rightTableName, rightTableAlias);
        return query;
    }
}
