﻿namespace Carbunql;

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

    /// <summary>
    /// Ensures that the enumerable source contains at least one element.
    /// Throws an <see cref="InvalidOperationException"/> if no elements are found.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable, must implement <see cref="IQuerySource"/>.</typeparam>
    /// <param name="source">The enumerable source to check.</param>
    /// <returns>The same enumerable source if it contains any elements.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no elements are found in the enumerable source.</exception>
    public static IEnumerable<T> EnsureAny<T>(this IEnumerable<T> source) where T : IQuerySource
    {
        if (source.Any())
            return source;

        throw new InvalidOperationException("No matching QuerySource was found.");
    }
}
