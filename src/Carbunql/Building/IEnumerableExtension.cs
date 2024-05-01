using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class IEnumerableExtension
{
    public static ValuesQuery ToValuesQuery<T>(this IEnumerable<T> source)
    {
        return ToValuesQuery(source, DbmsConfiguration.PlaceholderIdentifier);
    }

    public static ValuesQuery ToValuesQuery<T>(this IEnumerable<T> source, string placeholderIndentifer)
    {
        var vq = new ValuesQuery();
        var r = 0;
        foreach (var row in source)
        {
            var lst = new List<ValueBase>();
            var c = 0;
            foreach (var column in typeof(T).GetProperties().Where(x => x.CanRead).Select(x => x.GetValue(row)))
            {
                var name = $"r{r}c{c}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                vq.AddParameter(name, column);
                c++;
            }
            vq.Rows.Add(new ValueCollection(lst));
            r++;
        }
        return vq;
    }

    public static SelectQuery ToSelectQuery<T>(this IEnumerable<T> source)
    {
        return source.ToSelectQuery(DbmsConfiguration.PlaceholderIdentifier, x => x.ToLowerSnakeCase());
    }

    public static SelectQuery ToSelectQuery<T>(this IEnumerable<T> source, string placeholderIndentifer, Func<string, string> propertyNameConverter)
    {
        var columns = typeof(T).GetProperties().Where(x => x.CanRead).Select(x => propertyNameConverter(x.Name));
        var vq = source.ToValuesQuery(placeholderIndentifer);
        var sq = vq.ToSelectQuery(columns);

        return sq;
    }
}
