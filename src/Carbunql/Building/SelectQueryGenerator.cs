using Carbunql.Values;

namespace Carbunql.Building;

public static class SelectQueryGenerator
{
    public static SelectQuery FromObject(object obj, string parameterSufix = ":", Func<string, string>? nameFormatter = null)
    {
        return FromObject(obj, parameterSufix, nameFormatter, null);
    }

    private static SelectQuery FromObject(object obj, string parameterSufix, Func<string, string>? nameFormatter, int? index)
    {
        var sq = new SelectQuery();
        foreach (var item in obj.GetType().GetProperties().ToList().Where(x => x.CanRead))
        {
            var value = item.GetValue(obj);
            var name = item.Name;
            if (nameFormatter != null) name = nameFormatter(name);
            var key = parameterSufix + name;
            if (index != null) key = key + index;
            var c = new ParameterValue(key, value);
            sq.Select(c).As(name);
        }
        return sq;
    }

    public static SelectQuery FromObject<T>(IEnumerable<T> lst, string parameterSufix = ":", Func<string, string>? nameFormatter = null)
    {
        SelectQuery? sq = null;
        var index = 0;
        foreach (var item in lst)
        {
            if (item == null) continue;
            var tmp = FromObject(item, parameterSufix, nameFormatter, index);
            if (sq == null)
            {
                sq = tmp;
            }
            else
            {
                sq.UnionAll(tmp);
            }
            index++;
        }
        if (sq == null) throw new ArgumentException();
        return sq;
    }
}