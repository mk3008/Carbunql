﻿using Carbunql.Building;

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
    /// Gets the root query sources which are not referenced by any other query sources.
    /// </summary>
    public static IEnumerable<IQuerySource> GetRootsBySource(this IEnumerable<IQuerySource> querySources)
    {
        var maxLevel = querySources.Max(x => x.MaxLevel);
        var querySourceList = querySources.OrderByDescending(x => x.MaxLevel).ToList();
        var rootQuerySources = new List<IQuerySource>();

        var references = new List<int>();
        foreach (var item in querySourceList)
        {
            if (item.MaxLevel == maxLevel || !references.Contains(item.Index))
            {
                rootQuerySources.Add(item);
                references.AddRange(item.GetReferenceIndexes().ToList());
            }
        }
        return rootQuerySources;
    }

    /// <summary>
    /// Retrieves one QuerySource per query. The retrieval order is descending by Level and ascending by Sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source collection of QuerySources.</param>
    /// <returns>A collection of QuerySources, one per query, ordered by descending Level and ascending Sequence.</returns>
    public static IEnumerable<T> GetRootsByQuery<T>(this IEnumerable<T> source) where T : IQuerySource
    {
        var maxLevel = source.Max(x => x.MaxLevel);
        return source.Where(x => x.MaxLevel == maxLevel).GroupBy(ds => ds.Query).Select(ds => ds.OrderByDescending(s => s.MaxLevel).ThenBy(s => s.Index).First());
    }

    /// <summary>
    /// Ensures that the enumerable source contains at least one element.
    /// Throws an <see cref="InvalidOperationException"/> if no elements are found.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable, must implement <see cref="IQuerySource"/>.</typeparam>
    /// <param name="source">The enumerable source to check.</param>
    /// <returns>The same enumerable source if it contains any elements.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no elements are found in the enumerable source.</exception>
    public static IEnumerable<T> EnsureAny<T>(this IEnumerable<T> source, string appendErroressage = "") where T : IQuerySource
    {
        if (source.Any()) return source;

        if (string.IsNullOrEmpty(appendErroressage))
        {
            throw new InvalidOperationException($"No matching QuerySource was found.");
        }
        else
        {
            throw new InvalidOperationException($"No matching QuerySource was found. {appendErroressage}");
        }
    }
}
