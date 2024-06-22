using System.Collections;

namespace Carbunql.TypeSafe.Building;

internal class CollectionRegistory(IConstantRegistry innerManager) : IConstantRegistry
{
    public readonly IConstantRegistry InnerManager = innerManager;

    private List<string> Arguments { get; } = new();

    public IEnumerable<KeyValuePair<string, object>> Parameters => InnerManager.Parameters;

    public int Count() => InnerManager.Count();

    public string GetCollectionValue => string.Join(",", Arguments);

    public string Add(string key, object? collection)
    {
        if (collection != null && IsGenericList(collection.GetType()))
        {
            foreach (var item in (IEnumerable)collection)
            {
                Arguments.Add(ToParameter(item));
            }
        }
        return string.Empty;
    }

    private bool IsGenericList(Type? type)
    {
        if (type == null) return false;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
        {
            return true;
        }

        foreach (Type intf in type.GetInterfaces())
        {
            if (intf.IsGenericType && intf.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return true;
            }
        }

        return false;
    }

    private string ToParameter(object? value)
    {
        if (value == null)
        {
            return "null";
        }

        var tp = value.GetType();

        if (tp == typeof(string))
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return "''";
            }
            else
            {
                return InnerManager.Add(string.Empty, value);
            }
        }
        else if (tp == typeof(DateTime))
        {
            return InnerManager.Add(string.Empty, value);
        }
        else
        {
            return value.ToString()!;
        }
    }
}

