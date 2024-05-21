using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class ParameterExpressionExtension
{
    internal static string ToValue(this ParameterExpression prm
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        if (string.IsNullOrEmpty(prm.Name))
        {
            throw new Exception();
        }
        return addParameter(string.Empty, prm.Name);
    }
}
