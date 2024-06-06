using Carbunql.Building;

namespace Carbunql.TypeSafe;

internal class ParameterManager
{
    public ParameterManager(IEnumerable<QueryParameter> parameters, Func<string, object?, string> addParameterLogic)
    {
        Parameters = parameters.ToDictionary(x => x.ParameterName, x => x.Value);
        AddParameterLogic = addParameterLogic;
    }

    public Dictionary<string, object?> Parameters { get; set; }

    public Func<string, object?, string> AddParameterLogic { get; set; }

    /// <summary>
    /// Generates an available parameter name.
    /// The returned parameter name may not necessarily be unused.
    /// Even if a parameter name is already used, it can be reused if the value matches as well.
    /// </summary>
    /// <param name="variableName">Variable name</param>
    /// <param name="value">Variable value</param>
    /// <returns>Returns an available parameter name. It also indicates whether the parameter name is being reused.</returns>
    private Result GenerateParameterName(string variableName, object? value)
    {
        // If there is no variable name, and there is exactly one match with the same value, reuse it
        if (string.IsNullOrEmpty(variableName))
        {
            var q = Parameters.Where(x => x.Value != null && x.Value.Equals(value));
            if (q.Count() == 1)
            {
                var kv = q.First();
                return new Result(kv.Key, true);
            }
        }

        // Temporarily set the parameter name based on the variable name
        var requestKey = !string.IsNullOrEmpty(variableName) ? $"{DbmsConfiguration.PlaceholderIdentifier}{variableName.ToLowerSnakeCase()}" : $"{DbmsConfiguration.PlaceholderIdentifier}p{Parameters.Count}";

        // If the temporary parameter is unused, the desired parameter can be used as is
        if (!Parameters.ContainsKey(requestKey))
        {
            return new Result(requestKey, false);
        }

        var existingValue = Parameters[requestKey];

        // Even if used, if the existing parameter and key match the value, it can be reused
        if (existingValue != null && existingValue.Equals(value))
        {
            return new Result(requestKey, true);
        }

        // Otherwise, automatically assign a name.
        var index = 0;
        string tmpkey;
        do
        {
            tmpkey = $"{requestKey}_{index++}";
        } while (Parameters.ContainsKey(tmpkey));

        return new Result(tmpkey, false);
    }

    public string AddParameter(string key, object? value)
    {
        var nameInfo = GenerateParameterName(key, value);

        if (nameInfo.IsReuse) return nameInfo.ParameterName;

        AddParameterLogic(nameInfo.ParameterName, value);
        Parameters.Add(nameInfo.ParameterName, value);

        return nameInfo.ParameterName;
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