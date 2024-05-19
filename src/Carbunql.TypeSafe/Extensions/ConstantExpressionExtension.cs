using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class ConstantExpressionExtension
{
    internal static string ToValue(this ConstantExpression ce
        , Func<Expression, Func<object?, string>, string> mainConverter
        , Func<object?, string> addParameter)
    {
        var obj = ce.Value;
        var tp = ce.Type;

        if (obj == null)
        {
            return "null";
        }
        else if (tp == typeof(string))
        {
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return "''";
            }
            else
            {
                return addParameter(obj);
            }
        }
        else if (tp == typeof(DateTime))
        {
            return addParameter(obj);
        }
        else
        {
            return obj!.ToString()!;
        }
    }
}
