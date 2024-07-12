﻿using Carbunql.Clauses;
using Carbunql.Tables;
using System.Text;

namespace Carbunql;

/// <summary>
/// Represents a query source, which can be a physical table, subquery, or common table expression (CTE).
/// </summary>
public interface IQuerySource
{
    /// <summary>
    /// The index of the query source. It is a unique value within a query, starting from 1.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Gets the depth level of the query source.
    /// Numbering starts from 1 and increments with each nesting level.
    /// </summary>
    int MaxLevel { get; }

    /// <summary>
    /// Gets the alias name of the query source.
    /// </summary>
    string Alias { get; }

    /// <summary>
    /// Gets the column names belonging to the query source.
    /// Duplicates are removed, but order is not guaranteed.
    /// </summary>
    HashSet<string> ColumnNames { get; }

    /// <summary>
    /// Gets the select query to which the query source belongs.
    /// </summary>
    SelectQuery Query { get; }

    /// <summary>
    /// The selectable object that contains the query source.
    /// </summary>
    SelectableTable Source { get; }

    IList<IQuerySource> References { get; }
}

public static class IQuerySourceExtension
{
    public static string GetTableFullName(this IQuerySource querySource)
    {
        if (querySource.Source.Table is PhysicalTable pt)
        {
            return pt.GetTableFullName();
        }
        return string.Empty;
    }

    public static IQuerySource AddSourceComment(this IQuerySource querySource, string comment)
    {
        querySource.Source.CommentClause ??= new CommentClause();
        querySource.Source.CommentClause.Add(comment);
        return querySource;
    }

    public static IQuerySource AddQueryComment(this IQuerySource querySource, string comment)
    {
        querySource.Query.CommentClause ??= new CommentClause();
        querySource.Query.CommentClause.Add(comment);
        return querySource;
    }

    public static List<List<int>> ToTreePaths(this IQuerySource source)
    {
        var result = new List<List<int>>();
        BuildTreePaths(source, new List<int>(), result);
        return result;
    }

    private static void BuildTreePaths(IQuerySource source, List<int> currentPath, List<List<int>> result)
    {
        currentPath.Add(source.Index);

        if (source.References.Count == 0)
        {
            result.Add(new List<int>(currentPath));
        }
        else
        {
            foreach (var reference in source.References)
            {
                BuildTreePaths(reference, new List<int>(currentPath), result);
            }
        }
    }
}

