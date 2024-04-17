using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class ParameterValue : ValueBase
{
    public ParameterValue()
    {
        Key = null!;
    }

    public ParameterValue(string key)
    {
        Key = key;
    }

    public ParameterValue(string key, object? value)
    {
        Key = key;
        Value = value;
        Parameters.Add(new QueryParameter(key, value));
    }

    public string Key { get; set; }

    public object? Value { get; set; } = null;

    public List<QueryParameter> Parameters { get; set; } = new();

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return new Token(this, parent, Key);
    }

    protected override IEnumerable<QueryParameter> GetParametersCore()
    {
        return Parameters;
    }

    protected override IEnumerable<SelectQuery> GetInternalQueriesCore()
    {
        yield break;
    }

    protected override IEnumerable<PhysicalTable> GetPhysicalTablesCore()
    {
        yield break;
    }

    protected override IEnumerable<CommonTable> GetCommonTablesCore()
    {
        yield break;
    }
}
