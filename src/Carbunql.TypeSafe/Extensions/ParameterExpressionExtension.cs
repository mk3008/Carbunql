using Carbunql.TypeSafe.Building;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class ParameterExpressionExtension
{
    internal static string ToValue(this ParameterExpression prm, BuilderEngine engine)
    {
        if (string.IsNullOrEmpty(prm.Name))
        {
            throw new Exception();
        }
        return engine.AddParameter(prm.Name, null);
    }
}
