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

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery InnerJoin(this SelectQuery query, string leftAlias, FluentTable right, string condition)
    {
        return query.Join("inner join", right, condition);
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery InnerJoin(this SelectQuery query, string leftAlias, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("inner join", right, columns.ToCondition(leftAlias, right.Alias));
    }

    [Obsolete("The argument sequence has been corrected")]
    public static SelectQuery InnerJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        return query.InnerJoin(right, columns, left);
    }

    [Obsolete("use InnerJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns, FluentTable left)")]
    public static SelectQuery InnerJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        query.AddJoin("inner join", rightTableName, rightTableAlias, columns.ToCondition(leftTableAlias, rightTableAlias));
        return query;
    }

    public static SelectQuery InnerJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns)
    {
        var lquery = query.GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columns.Select(item => $"{lquery.GetColumn(item, isAliasIncluded: true)} = {rquery.GetColumn(item, isAliasIncluded: true)}"));

        return query.Join("inner join", right, condition);
    }

    public static SelectQuery InnerJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns, FluentTable left)
    {
        var lquery = new SelectQuery().From(left).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columns.Select(item => $"{lquery.GetColumn(item, isAliasIncluded: true)} = {rquery.GetColumn(item, isAliasIncluded: true)}"));

        return query.Join("inner join", right, condition);
    }

    public static SelectQuery InnerJoin(this SelectQuery query, FluentTable right, IEnumerable<string> rightcolumns, FluentTable left, IEnumerable<string> leftcolumns)
    {
        if (leftcolumns.Count() != rightcolumns.Count())
        {
            throw new ArgumentException("The number of elements in leftcolumns and rightcolumns must be the same.");
        }

        var columnMaps = leftcolumns.Zip(rightcolumns, (left, right) => (left, right));

        var lquery = new SelectQuery().From(left).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columnMaps.Select(item => $"{lquery.GetColumn(item.left, isAliasIncluded: true)} = {rquery.GetColumn(item.right, isAliasIncluded: true)}"));

        return query.Join("inner join", right, condition);
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery LeftJoin(this SelectQuery query, string leftAlias, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("left join", right, columns.ToCondition(leftAlias, right.Alias));
    }

    [Obsolete("use LeftJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns, FluentTable left)")]
    public static SelectQuery LeftJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("left join", right, columns.ToCondition(left.Alias, right.Alias));
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery LeftJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        return query.AddJoin("left join", rightTableName, rightTableAlias, columns.ToCondition(leftTableAlias, rightTableAlias));
    }

    public static SelectQuery LeftJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns)
    {
        var lquery = query.GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columns.Select(item => $"{lquery.GetColumn(item, isAliasIncluded: true)} = {rquery.GetColumn(item, isAliasIncluded: true)}"));

        return query.Join("left join", right, condition);
    }

    public static SelectQuery LeftJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns, FluentTable left)
    {
        var lquery = new SelectQuery().From(left).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columns.Select(item => $"{lquery.GetColumn(item, isAliasIncluded: true)} = {rquery.GetColumn(item, isAliasIncluded: true)}"));

        return query.Join("left join", right, condition);
    }

    public static SelectQuery LeftJoin(this SelectQuery query, FluentTable right, IEnumerable<string> rightcolumns, FluentTable left, IEnumerable<string> leftcolumns)
    {
        if (leftcolumns.Count() != rightcolumns.Count())
        {
            throw new ArgumentException("The number of elements in leftcolumns and rightcolumns must be the same.");
        }

        var columnMaps = leftcolumns.Zip(rightcolumns, (left, right) => (left, right));

        var lquery = new SelectQuery().From(left).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columnMaps.Select(item => $"{lquery.GetColumn(item.left, isAliasIncluded: true)} = {rquery.GetColumn(item.right, isAliasIncluded: true)}"));

        return query.Join("left join", right, condition);
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery RightJoin(this SelectQuery query, string leftAlias, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("right join", right, columns.ToCondition(leftAlias, right.Alias));
    }

    [Obsolete("use RightJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns, FluentTable left)")]
    public static SelectQuery RightJoin(this SelectQuery query, FluentTable left, FluentTable right, IEnumerable<string> columns)
    {
        return query.Join("right join", right, columns.ToCondition(left.Alias, right.Alias));
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery RightJoin(this SelectQuery query, string rightTableName, string rightTableAlias, string leftTableAlias, IEnumerable<string> columns)
    {
        return query.AddJoin("right join", rightTableName, rightTableAlias, columns.ToCondition(leftTableAlias, rightTableAlias));
    }

    public static SelectQuery RightJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns)
    {
        var lquery = query.GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columns.Select(item => $"{lquery.GetColumn(item, isAliasIncluded: true)} = {rquery.GetColumn(item, isAliasIncluded: true)}"));

        return query.Join("right join", right, condition);
    }

    public static SelectQuery RightJoin(this SelectQuery query, FluentTable right, IEnumerable<string> columns, FluentTable left)
    {
        var lquery = new SelectQuery().From(left).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columns.Select(item => $"{lquery.GetColumn(item, isAliasIncluded: true)} = {rquery.GetColumn(item, isAliasIncluded: true)}"));

        return query.Join("right join", right, condition);
    }

    public static SelectQuery RightJoin(this SelectQuery query, FluentTable right, IEnumerable<string> rightcolumns, FluentTable left, IEnumerable<string> leftcolumns)
    {
        if (leftcolumns.Count() != rightcolumns.Count())
        {
            throw new ArgumentException("The number of elements in leftcolumns and rightcolumns must be the same.");
        }

        var columnMaps = leftcolumns.Zip(rightcolumns, (left, right) => (left, right));

        var lquery = new SelectQuery().From(left).GetCurrentQuerySource();
        var rquery = new SelectQuery().From(right).GetCurrentQuerySource();

        var condition = string.Join(" and ", columnMaps.Select(item => $"{lquery.GetColumn(item.left, isAliasIncluded: true)} = {rquery.GetColumn(item.right, isAliasIncluded: true)}"));

        return query.Join("right join", right, condition);
    }

    public static SelectQuery CrossJoin(this SelectQuery query, FluentTable right)
    {
        return query.Join("cross join", right);
    }

    [Obsolete("Deprecated due to confusing argument definitions.")]
    public static SelectQuery CrossJoin(this SelectQuery query, string rightTableName, string rightTableAlias)
    {
        query.AddJoin("cross join", rightTableName, rightTableAlias);
        return query;
    }
}
