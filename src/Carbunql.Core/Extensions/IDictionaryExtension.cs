using System.Collections.Immutable;

namespace Carbunql.Core.Extensions;

internal static class IDictionaryExtension
{
    public static IDictionary<T1, T2> Merge<T1, T2>(this IEnumerable<IDictionary<T1, T2>> source) where T1 : notnull
    {
        IDictionary<T1, T2>? prm = null;
        foreach (var item in source)
        {
            prm ??= new Dictionary<T1, T2>();
            prm = prm.Merge(item);
        }
        if (prm != null) return prm;
        return ImmutableDictionary<T1, T2>.Empty;
    }

    public static IDictionary<T1, T2> Merge<T1, T2>(this IDictionary<T1, T2> source, IDictionary<T1, T2>? dic) where T1 : notnull
    {
        if (dic == null || !dic.Any()) return source;
        if (!source.Any()) return ImmutableDictionary<T1, T2>.Empty;

        var c = new Dictionary<T1, T2>();
        source.ForEach(x => c[x.Key] = x.Value);
        dic.ForEach(x => c[x.Key] = x.Value);
        return c;
    }

    public static void ForEach<T1, T2>(this IDictionary<T1, T2> source, Action<KeyValuePair<T1, T2>> action) where T1 : notnull
    {
        foreach (var x in source) action(x);
    }
}