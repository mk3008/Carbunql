﻿using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class GroupClauseExtension
{
    public static void Group(this SelectQuery source, FromClause from, string column)
    {
        source.Group(from.Root.Alias, column);
    }

    public static void Group(this SelectQuery source, SelectableTable table, string column)
    {
        source.Group(table.Alias, column);
    }

    public static void Group(this SelectQuery source, SelectableItem item)
    {
        source.GroupClause ??= new();
        source.GroupClause.Add(item.Value);
    }

    public static void Group(this SelectQuery source, string table, string column)
    {
        var item = new ColumnValue(table, column);
        source.GroupClause ??= new();
        source.GroupClause.Add(item);
    }
}