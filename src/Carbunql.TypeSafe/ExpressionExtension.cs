using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

internal static class ExpressionExtension
{
    internal static object? CompileAndInvoke(this NewExpression exp)
    {
        var delegateType = typeof(Func<>).MakeGenericType(exp.Type);
        var lambda = Expression.Lambda(delegateType, exp);
        var compiledLambda = lambda.Compile();
        return compiledLambda.DynamicInvoke();
    }


}