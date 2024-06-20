using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class ConstantExpressionExtension
{
    internal static string ToValue(this ConstantExpression ce, BuilderEngine engine)
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
                return engine.AddParameter(obj);
            }
        }
        else if (tp == typeof(DateTime))
        {
            return engine.AddParameter(obj);
        }
        else
        {
            return obj!.ToString()!;
        }
    }
}
