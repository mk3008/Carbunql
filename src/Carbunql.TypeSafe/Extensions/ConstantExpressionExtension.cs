using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class ConstantExpressionExtension
{
    internal static string ToValue(this ConstantExpression ce
        , Func<string, object?, string> addParameter)
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
                return addParameter(string.Empty, obj);
            }
        }
        else if (tp == typeof(DateTime))
        {
            return addParameter(string.Empty, obj);
        }
        else
        {
            return obj!.ToString()!;
        }
    }
}
