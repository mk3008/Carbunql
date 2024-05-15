using Carbunql.Analysis.Parser;
using Carbunql.Annotations;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

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


    public string ToValue(Expression exp, Func<object?, string> addParameter)
    {
        if (TryToValue(exp, addParameter, out var value))
        {
            return value;
        }
        throw new Exception();
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
                //var dbtype = DbmsConfiguration.ToDbType(tp);
                //return $"cast({obj} as {dbtype})";
                return obj!.ToString()!;
            }
        };

        if (exp is MemberExpression mem)
        {
            var tp = mem.Member.DeclaringType;

            if (tp == typeof(Sql))
            {
                if (mem.Member.Name == nameof(Sql.Now))
                {
                    value = DbmsConfiguration.GetNowCommandLogic();
                    return true;
                }
                if (mem.Member.Name == nameof(Sql.CurrentTimestamp))
                {
                    value = DbmsConfiguration.GetCurrentTimestampCommandLogic();
                    return true;
                }
                throw new InvalidProgramException();
            }
            if (mem.Expression is MemberExpression && typeof(ITableRowDefinition).IsAssignableFrom(tp))
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
            if (TryToValue(be.Left, addParameter, out var left) && TryToValue(be.Right, addParameter, out var right))
            {
                if (be.NodeType == ExpressionType.Coalesce)
                {
                    value = $"{DbmsConfiguration.CoalesceFunctionName}({left}, {right})";
                    return true;
                }
                if (be.NodeType == ExpressionType.Add)
                {
                    value = $"{left} + {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.Subtract)
                {
                    value = $"{left} - {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.Multiply)
                {
                    value = $"{left} * {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.Divide)
                {
                    value = $"{left} / {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.Modulo)
                {
                    value = DbmsConfiguration.GetModuloCommandLogic(left, right);
                    return true;
                }
                if (be.NodeType == ExpressionType.Equal)
                {
                    value = value = $"{left} = {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.NotEqual)
                {
                    value = value = $"{left} <> {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.GreaterThan)
                {
                    value = value = $"{left} > {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.GreaterThanOrEqual)
                {
                    value = value = $"{left} >= {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.LessThan)
                {
                    value = value = $"{left} < {right}";
                    return true;
                }
                if (be.NodeType == ExpressionType.LessThanOrEqual)
                {
                    value = value = $"{left} <= {right}";
                    return true;
                }
            }
        }
        else if (exp is UnaryExpression ue)
        {
            if (TryToValue(ue.Operand, addParameter, out var val))
            {
                if (ue.NodeType == ExpressionType.Convert)
                {
                    var dbtype = DbmsConfiguration.ToDbType(ue.Type);

                    ////excludes excessive casts
                    //if (val.StartsWith("cast("))
                    //{
                    //    var fv = (FunctionValue)ValueParser.Parse(val);
                    //    if (fv.Arguments.Count == 1 && fv.Arguments[0] is AsArgument arg && dbtype == arg.Type.ToText())
                    //    {
                    //        value = val;
                    //        return true;
                    //    }
                    //}

                    value = $"cast({val} as {dbtype})";
                    return true;
                }
            }
        }
        else if (exp is MethodCallExpression mce)
        {
            var args = mce.Arguments.Select(x => ToValue(x, addParameter));

            var tp = mce.Method.DeclaringType?.FullName;

            if (tp == typeof(Math).ToString())
            {
                if (mce.Method.Name == nameof(Math.Truncate))
                {
                    value = DbmsConfiguration.GetTruncateCommandLogic(args);
                    return true;
                }
                if (mce.Method.Name == nameof(Math.Floor))
                {
                    value = DbmsConfiguration.GetFloorCommandLogic(args);
                    return true;
                }
                if (mce.Method.Name == nameof(Math.Ceiling))
                {
                    value = DbmsConfiguration.GetCeilingCommandLogic(args);
                    return true;
                }
                if (mce.Method.Name == nameof(Math.Round))
                {
                    value = DbmsConfiguration.GetRoundCommandLogic(args);
                    return true;
                }
            }
            if (tp == typeof(Sql).ToString())
            {
                //reserved command
                if (mce.Method.Name == nameof(Sql.Raw))
                {
                    var arg = (ConstantExpression)mce.Arguments.First();
                    value = arg.Value!.ToString()!;
                    return true;
                }
                if (mce.Method.Name == nameof(Sql.RowNumber))
                {
                    if (mce.Arguments.Count == 0)
                    {
                        value = DbmsConfiguration.GetRowNumberCommandLogic();
                        return true;
                    }
                    if (mce.Arguments.Count == 2)
                    {
                        var argList = mce.Arguments.ToList();
                        var arg1st = (NewExpression)argList[0];
                        var arg2nd = (NewExpression)argList[1];
                        var arg1stText = string.Join(",", arg1st.Arguments.Select(x => ToValue(x, addParameter)));
                        var arg2ndText = string.Join(",", arg2nd.Arguments.Select(x => ToValue(x, addParameter)));

                        value = DbmsConfiguration.GetRowNumberPartitionByOrderByCommandLogic(arg1stText, arg2ndText);
                        return true;
                    }
                }
                if (mce.Method.Name == nameof(Sql.RowNumberOrderbyBy))
                {
                    var arg1st = (NewExpression)mce.Arguments.First();

                    var arg1stText = string.Join(",", arg1st.Arguments.Select(x => ToValue(x, addParameter)));

                    value = DbmsConfiguration.GetRowNumberOrderByCommandLogic(arg1stText);
                    return true;
                }
                if (mce.Method.Name == nameof(Sql.RowNumberPartitionBy))
                {
                    var arg1st = (NewExpression)mce.Arguments.First();

                    var arg1stText = string.Join(",", arg1st.Arguments.Select(x => ToValue(x, addParameter)));

                    value = DbmsConfiguration.GetRowNumberPartitionByCommandLogic(arg1stText);
                    return true;
                }
                throw new InvalidProgramException();
            }
        }
        else if (exp is ConditionalExpression cnd)
        {
            var test = ToValue(cnd.Test, addParameter);
            var ifTrue = ToValue(cnd.IfTrue, addParameter);
            var ifFalse = ToValue(cnd.IfFalse, addParameter);

            if (ifFalse.StartsWith("case "))
            {
                var caseExpression = CaseExpressionParser.Parse(ifFalse);
                if (caseExpression.CaseCondition is null)
                {
                    var we = WhenExpressionParser.Parse($"when {test} then {ifTrue}");
                    caseExpression.WhenExpressions.Insert(0, we);
                    value = caseExpression.ToText();
                    return true;
                }
                else
                {
                    value = $"case when {test} then {ifTrue} else {ifFalse} end";
                    return true;
                }
            }
            else
            {
                value = $"case when {test} then {ifTrue} else {ifFalse} end";
                return true;
            }
        }
        return false;
    }

    //private void Select(Type type, object? value, string alias, Func<string> getParameterName)
    //{
    //    if (value == null)
    //    {
    //        this.Select(new LiteralValue()).As(alias);
    //    }
    //    else if (type == typeof(string) || type == typeof(DateTime))
    //    {
    //        var pname = getParameterName();
    //        var val = AddParameter(DbmsConfiguration.PlaceholderIdentifier + pname, value);

    //        this.Select(val).As(alias);
    //    }
    //    else
    //    {
    //        var val = new LiteralValue(value.ToString());
    //        var dbtype = new LiteralValue(DbmsConfiguration.ToDbType(type));
    //        var arg = new AsArgument(val, dbtype);
    //        var functionValue = new FunctionValue("cast", arg);

    //        this.Select(functionValue).As(alias);
    //    }
    //}
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

    public static string Raw(string command)
    {
        return command;
    }

    public static string CurrentTimestamp => string.Empty;

    public static string Now => string.Empty;

    public static string RowNumber() => string.Empty;

    public static string RowNumber(object partition, object order) => string.Empty;

    public static string RowNumberPartitionBy(object partition) => string.Empty;

    public static string RowNumberOrderbyBy(object order) => string.Empty;
}