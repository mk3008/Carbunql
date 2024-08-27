using Carbunql.TypeSafe.Building;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class NewExpressionExtension
{
    internal static string ToValue(this NewExpression ne, BuilderEngine engine)
    {
        var obj = ne.CompileAndInvoke();
        var tp = ne.Type;

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

    internal static object? CompileAndInvoke(this NewExpression exp)
    {
        var delegateType = typeof(Func<>).MakeGenericType(exp.Type);
        var lambda = Expression.Lambda(delegateType, exp);
        var compiledLambda = lambda.Compile();
        return compiledLambda.DynamicInvoke();
    }
}
