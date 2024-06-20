using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class MemberExpressionExtension
{
    internal static string ToValue(this MemberExpression mem, BuilderEngine engine)
    {
        var tp = mem.Member.DeclaringType;

        if (tp == typeof(Sql))
        {
            // ex. Sql.Now, Sql.CurrentTimestamp
            return mem.CreateSqlCommand(engine);
        }
        if (mem.Expression is MemberExpression && typeof(IDataRow).IsAssignableFrom(tp))
        {
            //column
            var tableMember = (MemberExpression)mem.Expression;
            var table = tableMember.Member.Name;
            var column = mem.Member.Name;

            return $"{table}.{column}";
        }
        if (mem.Expression is ConstantExpression ce)
        {
            //variable
            return engine.AddParameter(mem.Member.Name, mem.CompileAndInvoke());
        }
        if (mem.Expression is MemberExpression me)
        {
            return me.ToValue(engine);
        }
        if (mem.Expression is ParameterExpression pe)
        {
            return $"{pe.Name}.{mem.Member.Name}";
        }
        throw new NotSupportedException($"Member.Name:{mem.Member.Name}, Member.DeclaringType:{mem.Member.DeclaringType}");
    }

    private static string CreateSqlCommand(this MemberExpression mem, BuilderEngine engine)
    {
        return mem.Member.Name switch
        {
            nameof(Sql.Now) => engine.SqlDialect.GetNowCommand(),
            nameof(Sql.CurrentTimestamp) => engine.SqlDialect.GetCurrentTimestampCommand(),
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
