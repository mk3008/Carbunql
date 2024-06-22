using Carbunql.Building;
using Carbunql.Extensions;
using Carbunql.TypeSafe.Dialect;
using System.Data;

namespace Carbunql.TypeSafe.Building;

internal class BuilderEngine(ISqlTranspiler sqlDialect, IConstantRegistry registory)
{
    internal readonly ISqlTranspiler SqlDialect = sqlDialect;
    public readonly IConstantRegistry Registory = registory;

    public string AddParameter(string name, object? value)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        var key = SqlDialect.ToParaemterName(name);
        var q = Registory.Parameters.Where(x => key.IsEqualNoCase(x.Key));
        if (!q.Any())
        {
            return Registory.Add(key, value);
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
        var q = Registory.Parameters.Where(x => x.Value != null && x.Value.Equals(value));
        if (q.Any())
        {
            return q.First().Key;
        }
        else
        {
            var name = $"const_{Registory.Parameters.Count()}";
            return AddParameter(name, value);
        }

        throw new InvalidProgramException();
    }
}
