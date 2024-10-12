namespace Carbunql.Fluent;

public static class SelectQueryJoinExtensions
{
    private static string ToCondition(this IEnumerable<string> columns, string leftAlias, string rightAlias)
    {
        return string.Join(" and ", columns.Select(x => $"{leftAlias}.{x} = {rightAlias}.{x}"));
    }

    private static SelectQuery Join(this SelectQuery query, string joinCommand, FluentTable right, string condition = "")
    {
        if (right.IsCommonTable)
        {
            query.With(right);
        }

        if (!string.IsNullOrEmpty(condition))
        {
            query.AddJoin(joinCommand, right.ToSelectable(), condition);
        }
        else
        {
            query.AddJoin(joinCommand, right.ToSelectable());
        }

        return query;
    }

    public static SelectQuery InnerJoin(this SelectQuery query, string leftAlias, FluentTable right, string condition)
    {
        return query.Join("inner join", right, condition);
    }

    public static SelectQuery InnerJoin(this SelectQuery query, string leftAlias, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("inner join", right, columns.ToCondition(leftAlias, right.Alias));
    }

    public static SelectQuery InnerJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        return query.InnerJoin(left.Alias, right, columns.ToCondition(left.Alias, right.Alias));
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery InnerJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        query.AddJoin("inner join", rightTableName, rightTableAlias, columns.ToCondition(leftTableAlias, rightTableAlias));
        return query;
    }

    public static SelectQuery LeftJoin(this SelectQuery query, string leftAlias, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("left join", right, columns.ToCondition(leftAlias, right.Alias));
    }

    public static SelectQuery LeftJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("left join", right, columns.ToCondition(left.Alias, right.Alias));
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery LeftJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        return query.AddJoin("left join", rightTableName, rightTableAlias, columns.ToCondition(leftTableAlias, rightTableAlias));
    }

    public static SelectQuery RightJoin(this SelectQuery query, string leftAlias, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("right join", right, columns.ToCondition(leftAlias, right.Alias));
    }

    public static SelectQuery RightJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("right join", right, columns.ToCondition(left.Alias, right.Alias));
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery RightJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        return query.AddJoin("right join", rightTableName, rightTableAlias, columns.ToCondition(leftTableAlias, rightTableAlias));
    }

    public static SelectQuery CrossJoin(this SelectQuery query, FluentTable right)
    {
        return query.Join("cross join", right);
    }

    public static SelectQuery CrossJoin(this SelectQuery query, string rightTableName, string rightTableAlias)
    {
        query.AddJoin("cross join", rightTableName, rightTableAlias);
        return query;
    }
}
