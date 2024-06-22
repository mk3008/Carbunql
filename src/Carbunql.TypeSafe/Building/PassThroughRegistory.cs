namespace Carbunql.TypeSafe.Building;

internal class PassThroughRegistory() : IConstantRegistry
{
    public IEnumerable<KeyValuePair<string, object>> Parameters => throw new NotImplementedException();

    public string Add(string key, object? value)
    {
        return value is null ? "null" : value.ToString()!;
    }

    public int Count()
    {
        throw new NotImplementedException();
    }
}

