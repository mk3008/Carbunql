using MessagePack;

namespace Carbunql;

[MessagePackObject(keyAsPropertyName: true)]
public class QueryParameter
{
    public QueryParameter(string parameterName, object? value)
    {
        ParameterName = parameterName;
        Value = value;
    }

    public string ParameterName { get; set; }

    public object? Value { get; set; }
}
