using Carbunql.Building;
using System.Collections;

namespace Carbunql.TypeSafe;

internal interface IParameterManager
{
    string AddParameter(string key, object? value);

    int Count();

    IEnumerable<KeyValuePair<string, object>> Parameters { get; }
}

internal class StringFormatParameterManager(IParameterManager innerManager) : IParameterManager
{
    public readonly IParameterManager InnerManager = innerManager;

    public IEnumerable<KeyValuePair<string, object>> Parameters => InnerManager.Parameters;

    public int Count() => InnerManager.Count();

    public string AddParameter(string key, object? value)
    {
        var v = ConverToDbDateFormat(value!.ToString()!);
        return InnerManager.AddParameter(key, v);
    }

    private string ConverToDbDateFormat(string csharpFormat)
    {
        var replacements = new Dictionary<string, string>
        {
            {"yyyy", "YYYY"},
            {"MM", "MM"},
            {"dd", "DD"},
            {"HH", "HH24"},
            {"mm", "MI"},
            {"ss", "SS"},
            {"ffffff", "US"},
            {"fff", "MS"}
        };

        string dbformat = csharpFormat;

        foreach (var pair in replacements)
        {
            dbformat = dbformat.Replace(pair.Key, pair.Value);
        }

        return dbformat;
    }
}


internal class EchoParameterManager() : IParameterManager
{
    public IEnumerable<KeyValuePair<string, object>> Parameters => throw new NotImplementedException();

    public string AddParameter(string key, object? value)
    {
        return value is null ? "null" : value.ToString()!;
    }

    public int Count()
    {
        throw new NotImplementedException();
    }
}

internal class CollectionParameterManager(IParameterManager innerManager) : IParameterManager
{
    public readonly IParameterManager InnerManager = innerManager;

    private List<string> Arguments { get; } = new();

    public IEnumerable<KeyValuePair<string, object>> Parameters => InnerManager.Parameters;

    public int Count() => InnerManager.Count();

    public string GetCollectionValue => string.Join(",", Arguments);

    public string AddParameter(string key, object? collection)
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

    private string ToParameter(Object? value)
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
                return InnerManager.AddParameter(string.Empty, value);
            }
        }
        else if (tp == typeof(DateTime))
        {
            return InnerManager.AddParameter(string.Empty, value);
        }
        else
        {
            return value.ToString()!;
        }
    }
}

internal class ArrayParameterManager(BuilderEngine innerEngine, string variableName, string arrayParameterName) : IParameterManager
{
    public readonly IParameterManager InnerManager = innerEngine.Manager;

    public readonly string VariableName = innerEngine.SqlDialect.ToParaemterName(variableName);

    public readonly string ArrayParameterName = arrayParameterName;

    public bool IsCommandCreated { get; private set; } = false;

    public IEnumerable<KeyValuePair<string, object>> Parameters => InnerManager.Parameters;

    public string AddParameter(string key, object? value)
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

internal class ParameterManager : IParameterManager
{
    public ParameterManager(IEnumerable<QueryParameter> parameters, Func<string, object?, string> addParameterLogic)
    {
        Parameters = parameters.ToDictionary(x => x.ParameterName, x => x.Value);
        AddParameterLogic = addParameterLogic;
    }

    public Dictionary<string, object?> Parameters { get; set; }

    public Func<string, object?, string> AddParameterLogic { get; set; }

    IEnumerable<KeyValuePair<string, object>> IParameterManager.Parameters => Parameters!;

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

    public string AddParameter(string key, object? value)
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

