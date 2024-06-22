namespace Carbunql.TypeSafe.Building;

internal interface IConstantRegistry
{
    string Add(string key, object? value);

    int Count();

    IEnumerable<KeyValuePair<string, object>> Parameters { get; }
}
