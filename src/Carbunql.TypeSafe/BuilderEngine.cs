using Carbunql.Building;
using Carbunql.Extensions;
using System.Data;

namespace Carbunql.TypeSafe;

internal class BuilderEngine(ISqlDialect sqlDialect, IParameterManager manager)
{
    internal readonly ISqlDialect SqlDialect = sqlDialect;
    public readonly IParameterManager Manager = manager;

    //public IEnumerable<KeyValuePair<string, object>> Parameters => Manager.Parameters;

    public string AddParameter(string name, object? value)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        var key = SqlDialect.ToParaemterName(name);
        var q = Manager.Parameters.Where(x => key.IsEqualNoCase(x.Key));
        if (!q.Any())
        {
            return Manager.AddParameter(key, value);
        }
        else if (q.First().Value.Equals(value))
        {
            return key;
        }

        throw new InvalidProgramException();
    }

    /// <summary>
    /// Anonymous constant value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidProgramException"></exception>
    public string AddParameter(object value)
    {
        var q = Manager.Parameters.Where(x => x.Value != null && x.Value.Equals(value));
        if (q.Any())
        {
            return q.First().Key;
        }
        else
        {
            var name = $"const_{Manager.Parameters.Count()}";
            return AddParameter(name, value);
        }

        throw new InvalidProgramException();
    }

    //public int Count()
    //{
    //    return Manager.Count();
    //}
}
