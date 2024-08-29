using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;

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

    /// <summary>
    /// Gets the query sources that reference this query source.
    /// </summary>
    IList<IQuerySource> References { get; }

    SourceType SourceType { get; }
}

public enum SourceType
{
    PhysicalTable,
    SubQuery,
    CommonTableExtension,
    ValuesQuery
}

public static class IQuerySourceExtension
{
    public static void AddComment(this IQuerySource querySource, string comment)
    {
        querySource.Source.AddComment(comment);
    }

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

        //set root querysource(index:0)
        result.ForEach(x => x.Add(0));

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

    public static HashSet<int> GetReferenceIndexes(this IQuerySource source)
    {
        var q0 = new int[] { source.Index };
        var q1 = source.References.Select(x => x.Index);
        var q2 = source.References.SelectMany(x => x.References).SelectMany(x => x.GetReferenceIndexes());

        var q = q0.Union(q1).Union(q2);
        var lst = q.ToHashSet();
        return lst;
    }
}

