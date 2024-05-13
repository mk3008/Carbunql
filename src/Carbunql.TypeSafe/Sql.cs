using Carbunql.Annotations;
using Carbunql.Clauses;
using Carbunql.Building;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Carbunql.Values;
using System.Data.Common;
using System.Reflection;

namespace Carbunql.TypeSafe;

public interface ITableRowDefinition
{
    [IgnoreMapping]
    TableDefinitionClause TableDefinition { get; set; }
}

public class FluentSelectQuery : SelectQuery
{
    public FluentSelectQuery Select<T>(Expression<Func<T>> expression) where T : class
    {
        var analyzed = ExpressionReader.Analyze(expression);

        var body = (NewExpression)expression.Body;

        /*
* LambdaExpression
NodeType
    Lambda
Type
    Func`1
Name
    ""
ReturnType
    <>f__AnonymousType0`1[System.Int32]
Body
    * NewExpression
    NodeType
        New
    Type
        <>f__AnonymousType0`1
    Arguments.Count
        1
    - index : 0
        * MemberExpression
        NodeType
            MemberAccess
        Type
            Int32
        Member
            ** MemberInfo
            Name
                sale_id
            MemberType
                Property
        Expression
            * MemberExpression
            NodeType
                MemberAccess
            Type
                sale
            Member
                ** MemberInfo
                Name
                    a
                MemberType
                    Field
            Expression
                * ConstantExpression
                NodeType
                    Constant
                Type
                    <>c__DisplayClass4_0
                Value
                    Carbunql.TypeSafe.Test.SingleTableTest+<>c__DisplayClass4_0
    Members.Count
        1
    - index : 0
        ** MemberInfo
        Name
            id
        MemberType
            Property
Parameters count
    0
         */

        var parameterCount = this.GetParameters().Count();
        Func<object?, string> addParameter = (obj) =>
        {
            var pname = $"{DbmsConfiguration.PlaceholderIdentifier}p{parameterCount}";
            parameterCount++;
            AddParameter(pname, obj);
            return pname;
        };

        if (body.Members != null)
        {
            var cnt = body.Members.Count();
            for (var i = 0; i < cnt; i++)
            {
                var alias = body.Members[i].Name;

                if (TryToValue(body.Arguments[i], addParameter, out var value))
                {
                    this.Select(value).As(alias);
                }

            }
        }

        return this;
    }

    public bool TryToValue(Expression exp, Func<object?, string> addParameter, out string value)
    {
        value = string.Empty;

        //var type = exp.Type;

        Func<object?, Type, string> fn = (obj, tp) =>
        {
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
                var dbtype = DbmsConfiguration.ToDbType(tp);
                return $"cast({obj} as {dbtype})";
            }
        };

        if (exp is MemberExpression mem)
        {
            if (mem.Expression is MemberExpression && typeof(ITableRowDefinition).IsAssignableFrom(mem.Expression.Type))
            {
                //column
                var table = ((MemberExpression)mem.Expression).Member.Name;
                var column = mem.Member.Name;
                value = $"{table}.{column}";
                return true;
            }
            else if (mem.Expression is ConstantExpression ce)
            {
                //variable
                var val = mem.CompileAndInvoke();
                value = fn(val, mem.Type);
                return true;
            }
        }
        else if (exp is ConstantExpression ce)
        {
            // static value
            value = fn(ce.Value, ce.Type);
            return true;
        }
        else if (exp is NewExpression ne)
        {
            // ex. new Datetime
            value = fn(ne.CompileAndInvoke(), ne.Type);
            return true;
        }
        else if (exp is BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Coalesce)
            {

                if (TryToValue(be.Left, addParameter, out var left) && TryToValue(be.Right, addParameter, out var right))
                {
                    value = $"{DbmsConfiguration.CoalesceFunctionName}({left}, {right})";
                    return true;
                }
            }

        }
        return false;
    }

    private void Select(Type type, object? value, string alias, Func<string> getParameterName)
    {
        if (value == null)
        {
            this.Select(new LiteralValue()).As(alias);
        }
        else if (type == typeof(string) || type == typeof(DateTime))
        {
            var pname = getParameterName();
            var val = AddParameter(DbmsConfiguration.PlaceholderIdentifier + pname, value);

            this.Select(val).As(alias);
        }
        else
        {
            var val = new LiteralValue(value.ToString());
            var dbtype = new LiteralValue(DbmsConfiguration.ToDbType(type));
            var arg = new AsArgument(val, dbtype);
            var functionValue = new FunctionValue("cast", arg);

            this.Select(functionValue).As(alias);
        }
    }

}

internal static class ExpressionExtension
{
    //internal static object? CompileAndInvoke(this MemberExpression exp)
    //{
    //    var method = typeof(ExpressionExtension)
    //        .GetMethod(nameof(CompileAndInvokeCore), BindingFlags.NonPublic | BindingFlags.Static)!
    //        .MakeGenericMethod(exp.Type);

    //    return method.Invoke(null, new object[] { exp });
    //}

    //internal static T CompileAndInvokeCore<T>(this MemberExpression exp)
    //{
    //    var lm = Expression.Lambda<Func<T>>(exp);
    //    return lm.Compile().Invoke();
    //}

    internal static object? CompileAndInvoke(this NewExpression exp)
    {
        var delegateType = typeof(Func<>).MakeGenericType(exp.Type);
        var lambda = Expression.Lambda(delegateType, exp);
        var compiledLambda = lambda.Compile();
        return compiledLambda.DynamicInvoke();
    }

    internal static object? CompileAndInvoke(this MemberExpression exp)
    {
        var delegateType = typeof(Func<>).MakeGenericType(exp.Type);
        var lambda = Expression.Lambda(delegateType, exp);
        var compiledLambda = lambda.Compile();
        return compiledLambda.DynamicInvoke();
    }
}

/// <summary>
/// Only function definitions are written for use in expression trees.
/// The actual situation is in ExpressionExtension.cs.
/// </summary
public static class Sql
{
    public static T DefineTable<T>() where T : ITableRowDefinition, new()
    {
        var instance = new T();
        instance.TableDefinition = TableDefinitionClauseFactory.Create<T>();
        return instance;
    }

    public static FluentSelectQuery From<T>(Expression<Func<T>> expression) where T : ITableRowDefinition
    {
        var sq = new FluentSelectQuery();

        var alias = ((MemberExpression)expression.Body).Member.Name;

        //execute
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        sq.From(result.TableDefinition).As(alias);

        return sq;
    }
}