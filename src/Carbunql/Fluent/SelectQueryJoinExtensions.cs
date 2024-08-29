namespace Carbunql.Fluent;

public static class SelectQueryJoinExtensions
{
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
