namespace Carbunql.TypeSafe.Building;

internal class ArrayRegistry(BuilderEngine innerEngine, string variableName, string arrayParameterName) : IConstantRegistry
{
    public readonly IConstantRegistry InnerManager = innerEngine.Registory;

    public readonly string VariableName = innerEngine.SqlDialect.ToParaemterName(variableName);

    public readonly string ArrayParameterName = arrayParameterName;

    public bool IsCommandCreated { get; private set; } = false;

    public IEnumerable<KeyValuePair<string, object>> Parameters => InnerManager.Parameters;

    public string Add(string key, object? value)
    {
        if (VariableName.Equals(key))
        {
            IsCommandCreated = true;
            return $"any({ArrayParameterName})";
        }
        throw new InvalidProgramException();
    }

    public int Count() => InnerManager.Count();
}

