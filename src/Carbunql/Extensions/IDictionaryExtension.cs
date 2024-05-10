namespace Carbunql.Extensions;

/// <summary>
/// Provides extension methods for the IDictionary interface.
/// </summary>
internal static class IDictionaryExtension
{
    /// <summary>
    /// Merges two dictionaries, checking for duplicate keys.
    /// If a duplicate key is found with different values, an exception is thrown.
    /// </summary>
    /// <typeparam name="T1">The type of keys in the dictionaries.</typeparam>
    /// <typeparam name="T2">The type of values in the dictionaries.</typeparam>
    /// <param name="source">The first dictionary.</param>
    /// <param name="dic">The second dictionary to merge.</param>
    /// <returns>A new dictionary containing the merged key-value pairs.</returns>
    /// <exception cref="ArgumentException">Thrown when a duplicate key is found with different values.</exception>
    [Obsolete]
    public static IDictionary<T1, T2> Merge<T1, T2>(this IDictionary<T1, T2> source, IDictionary<T1, T2>? dic) where T1 : notnull
    {
        if (dic == null || !dic.Any()) return source;

        var mergedDictionary = new Dictionary<T1, T2>();
        source.ForEach(x => mergedDictionary[x.Key] = x.Value);
        dic.ForEach(x =>
        {
            if (mergedDictionary.ContainsKey(x.Key))
            {
                if (!EqualityComparer<T2>.Default.Equals(mergedDictionary[x.Key], x.Value))
                {
                    throw new ArgumentException($"Duplicate key '{x.Key}' with different values.");
                }
            }
            else
            {
                mergedDictionary[x.Key] = x.Value;
            }
        });

        return mergedDictionary;
    }

    /// <summary>
    /// Performs the specified action on each element of the dictionary.
    /// </summary>
    /// <typeparam name="T1">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="T2">The type of values in the dictionary.</typeparam>
    /// <param name="source">The dictionary to iterate over.</param>
    /// <param name="action">The action to perform on each element of the dictionary.</param>
    public static void ForEach<T1, T2>(this IDictionary<T1, T2> source, Action<KeyValuePair<T1, T2>> action) where T1 : notnull
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
}
