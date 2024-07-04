namespace Carbunql;

public static class IEnumerableExtension
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
        {
            action(item);
        }
        return items;
    }

    /// <summary>
    /// Retrieves one QuerySource per branch. The retrieval order is descending by Level and ascending by Sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source collection of QuerySources.</param>
    /// <returns>A collection of QuerySources, one per branch, ordered by descending Level and ascending Sequence.</returns>
    public static IEnumerable<T> GetRootsByBranch<T>(this IEnumerable<T> source) where T : IQuerySource
    {
        return source.GroupBy(ds => ds.Branch).Select(ds => ds.OrderByDescending(s => s.Level).ThenBy(s => s.Sequence).First());
    }

    /// <summary>
    /// Retrieves one QuerySource per query. The retrieval order is descending by Level and ascending by Sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source collection of QuerySources.</param>
    /// <returns>A collection of QuerySources, one per query, ordered by descending Level and ascending Sequence.</returns>
    public static IEnumerable<T> GetRootsByQuery<T>(this IEnumerable<T> source) where T : IQuerySource
    {
        return source.GroupBy(ds => ds.Query).Select(ds => ds.OrderByDescending(s => s.Level).ThenBy(s => s.Sequence).First());
    }
}
