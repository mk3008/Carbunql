using Carbunql.Building;

namespace Carbunql.TypeSafe.Building;

internal class ParameterRegistory : IConstantRegistry
{
    public ParameterRegistory(IEnumerable<QueryParameter> parameters, Func<string, object?, string> addParameterLogic)
    {
        Parameters = parameters.ToDictionary(x => x.ParameterName, x => x.Value);
        AddParameterLogic = addParameterLogic;
    }

    public Dictionary<string, object?> Parameters { get; set; }

    public Func<string, object?, string> AddParameterLogic { get; set; }

    IEnumerable<KeyValuePair<string, object>> IConstantRegistry.Parameters => Parameters!;

    /// <summary>
    /// Generates an available parameter name.
    /// The returned parameter name may not necessarily be unused.
    /// Even if a parameter name is already used, it can be reused if the value matches as well.
    /// </summary>
    /// <param name="variableName">Variable name</param>
    /// <param name="value">Variable value</param>
    /// <returns>Returns an available parameter name. It also indicates whether the parameter name is being reused.</returns>
    private Result GenerateParameterName(string key, object? value)
    {
        // If there is no variable name, and there is exactly one match with the same value, reuse it
        if (string.IsNullOrEmpty(key))
        {
            var q = Parameters.Where(x => x.Value != null && x.Value.Equals(value));
            if (q.Count() == 1)
            {
                var kv = q.First();
                return new Result(kv.Key, true);
            }
        }

        // If the temporary parameter is unused, the desired parameter can be used as is
        if (!Parameters.ContainsKey(key))
        {
            return new Result(key, false);
        }

        var existingValue = Parameters[key];

        // Even if used, if the existing parameter and key match the value, it can be reused
        if (existingValue != null && existingValue.Equals(value))
        {
            return new Result(key, true);
        }

        // Otherwise, automatically assign a name.
        var index = 0;
        string tmpkey;
        do
        {
            tmpkey = $"{key}_{index++}";
        } while (Parameters.ContainsKey(tmpkey));

        return new Result(tmpkey, false);
    }

    public string Add(string key, object? value)
    {
        var nameInfo = GenerateParameterName(key, value);

        if (nameInfo.IsReuse) return nameInfo.ParameterName;

        AddParameterLogic(nameInfo.ParameterName, value);
        Parameters.Add(nameInfo.ParameterName, value);

        return nameInfo.ParameterName;
    }

    public int Count()
    {
        return Parameters.Count();
    }

    internal struct Result
    {
        public string ParameterName { get; }
        public bool IsReuse { get; }

        public Result(string parameterName, bool isReuse)
        {
            ParameterName = parameterName;
            IsReuse = isReuse;
        }
    }
}

