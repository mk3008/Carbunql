using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class MemberExpressionExtension
{
    internal static string ToValue(this MemberExpression mem
        , Func<Expression, Func<object?, string>, string> mainConverter
        , Func<object?, string> addParameter)
    {
        var tp = mem.Member.DeclaringType;

        if (tp == typeof(Sql))
        {
            // ex. Sql.Now, Sql.CurrentTimestamp
            return CreateSqlCommand(mem);
        }
        if (mem.Expression is MemberExpression && typeof(ITableRowDefinition).IsAssignableFrom(tp))
        {
            //column
            var table = ((MemberExpression)mem.Expression).Member.Name;
            var column = mem.Member.Name;
            return $"{table}.{column}";
        }
        if (mem.Expression is ConstantExpression ce)
        {
            //variable
            return addParameter(mem.CompileAndInvoke());
        }

        throw new NotSupportedException($"Member.Name:{mem.Member.Name}, Member.DeclaringType:{mem.Member.DeclaringType}");
    }

    private static string CreateSqlCommand(MemberExpression mem)
    {
        return mem.Member.Name switch
        {
            nameof(Sql.Now) => FluentSelectQuery.CreateCastStatement(DbmsConfiguration.GetNowCommandLogic(), typeof(DateTime)),
            nameof(Sql.CurrentTimestamp) => DbmsConfiguration.GetCurrentTimestampCommandLogic(),
            _ => throw new NotSupportedException($"The member '{mem.Member.Name}' is not supported.")
        };
    }

    internal static object? CompileAndInvoke(this MemberExpression exp)
    {
        var delegateType = typeof(Func<>).MakeGenericType(exp.Type);
        var lambda = Expression.Lambda(delegateType, exp);
        var compiledLambda = lambda.Compile();
        return compiledLambda.DynamicInvoke();
    }
}
