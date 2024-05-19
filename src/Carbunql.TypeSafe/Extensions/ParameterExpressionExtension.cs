using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class ParameterExpressionExtension
{
    internal static string ToValue(this ParameterExpression prm
        , Func<Expression, Func<object?, string>, string> mainConverter
        , Func<object?, string> addParameter)
    {
        if (string.IsNullOrEmpty(prm.Name))
        {
            throw new Exception();
        }
        return addParameter(prm.Name);
    }
}
