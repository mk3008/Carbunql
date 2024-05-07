using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides methods for generating a collection of parameter values from objects.
/// </summary>
public static class ValueCollectionGenerator
{
    /// <summary>
    /// Generates a collection of parameter values from the properties of the specified object.
    /// </summary>
    /// <param name="obj">The object from which to extract property values.</param>
    /// <param name="properties">The list of property names to extract values from.</param>
    /// <param name="paramSufix">The suffix to append to parameter keys.</param>
    /// <param name="keyFormatter">Optional. A function to format property names into parameter keys.</param>
    /// <param name="index">Optional. An index to append to parameter keys.</param>
    /// <returns>A collection of parameter values extracted from the object's properties.</returns>
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
