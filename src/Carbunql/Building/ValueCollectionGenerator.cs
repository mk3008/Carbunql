using Carbunql.Values;

namespace Carbunql.Building;

public static class ValueCollectionGenerator
{
    public static ValueCollection FromObject(object obj, List<string> properties, string paramSufix, Func<string, string>? keyFormatter, int? index)
    {
        keyFormatter ??= (string x) => x;

        var vc = new ValueCollection();
        foreach (var item in properties)
        {
            var prop = obj.GetType().GetProperty(item);
            if (prop == null) throw new NullReferenceException();

            var value = prop.GetValue(obj);
            var name = keyFormatter(prop.Name);
            var key = paramSufix + name;
            if (index != null) key = key + index;
            var c = new ParameterValue(key, value);
            vc.Add(c);
        }
        return vc;
    }
}