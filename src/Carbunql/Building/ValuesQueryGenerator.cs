using Carbunql.Values;

namespace Carbunql.Building;

public static class ValuesQueryGenerator
{
    public static ValuesQuery FromItem<T>(T item, List<string> props, string parameterSufix = ":", Func<string, string>? keyFormatter = null)
    {
        return FromList<T>(new[] { item }, props, parameterSufix, keyFormatter);
    }

    public static ValuesQuery FromList<T>(IEnumerable<T> lst, List<string> props, string parameterSufix = ":", Func<string, string>? keyFormatter = null)
    {
        keyFormatter ??= (string x) => x;

        var rows = new List<ValueCollection>();
        var index = 0;
        foreach (var item in lst)
        {
            if (item == null) continue;
            rows.Add(ValueCollectionGenerator.FromObject(item, props, parameterSufix, keyFormatter, index));
            index++;
        }
        return new ValuesQuery(rows);
    }
}