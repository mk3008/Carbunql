using System.Collections.Immutable;

namespace Carbunql.Extensions;

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
	/// <exception cref="ArgumentException"></exception>
	public static IDictionary<T1, T2> Merge<T1, T2>(this IDictionary<T1, T2> source, IDictionary<T1, T2>? dic) where T1 : notnull
	{
		if (dic == null || !dic.Any()) return source;

		var c = new Dictionary<T1, T2>();
		source.ForEach(x => c[x.Key] = x.Value);
		dic.ForEach(x =>
		{
			if (c.ContainsKey(x.Key))
			{
				if (!EqualityComparer<T2>.Default.Equals(c[x.Key], x.Value))
				{
					throw new ArgumentException($"Duplicate key '{x.Key}' with different values.");
				}
			}
			else
			{
				c[x.Key] = x.Value;
			}
		});

		return c;
	}

	public static void ForEach<T1, T2>(this IDictionary<T1, T2> source, Action<KeyValuePair<T1, T2>> action) where T1 : notnull
	{
		foreach (var x in source) action(x);
	}
}