using Carbunql.Values;

namespace Carbunql.Building;

public static class SelectQueryGenerator
{
    public static SelectQuery FromObject(object obj, string parameterSufix = ":", Func<string, string>? nameFormatter = null)
    {
        var sq = new SelectQuery();
        foreach (var item in obj.GetType().GetProperties().ToList().Where(x => x.CanRead))
        {
            var value = item.GetValue(obj);
            var name = item.Name;
            if (nameFormatter != null) name = nameFormatter(name);
            var key = parameterSufix + name;
            var c = new ParameterValue(key, value);
            sq.Select(c).As(name);
        }
        return sq;
    }
}