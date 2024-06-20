using Carbunql.Analysis.Parser;
using Carbunql.Annotations;
using Carbunql.Building;
using Carbunql.Extensions;
using Carbunql.TypeSafe.Extensions;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class MethodCallExpressionExtension
{
    internal static string ToValue(this MethodCallExpression mce, BuilderEngine engine)
    {
        // DeclaringType
        if (mce.Method.DeclaringType == typeof(Math))
        {
            return CreateMathCommand(mce, engine);
        }
        if (mce.Method.DeclaringType == typeof(Sql))
        {
            return CreateSqlCommand(mce, engine);
        }
        if (mce.Method.DeclaringType == typeof(DateTimeExtension))
        {
            return CreateDateTimeExtensionCommand(mce, engine);
        }

        // Return Type
        if (mce.Type == typeof(string))
        {
            return ToStringValue(mce, engine);
        }
        if (mce.Type == typeof(bool))
        {
            return ToBoolValue(mce, engine);
        }
        if (mce.Type == typeof(DateTime))
        {
            return ToDateTimeValue(mce, engine);
        }

        throw new NotSupportedException($"Type:{mce.Type}, Method.DeclaringType:{mce.Method.DeclaringType}");
    }

    private static string CreateMathCommand(this MethodCallExpression mce, BuilderEngine engine)
    {
        var args = mce.Arguments.Select(x => RemoveRootBracketOrDefault(x.ToValue(engine)));

        return mce.Method.Name switch
        {
            nameof(Math.Truncate) => engine.SqlDialect.GetTruncateCommand(args),
            nameof(Math.Floor) => engine.SqlDialect.GetFloorCommand(args),
            nameof(Math.Ceiling) => engine.SqlDialect.GetCeilingCommand(args),
            nameof(Math.Round) => engine.SqlDialect.GetRoundCommand(args),
            _ => throw new NotSupportedException($"The method '{mce.Method.Name}' is not supported.")
        };
    }

    private static string CreateDateTimeExtensionCommand(MethodCallExpression mce, BuilderEngine engine)
    {
        switch (mce.Method.Name)
        {
            case nameof(DateTimeExtension.TruncateToYear):
                return $"date_trunc('year', {mce.Arguments[0].ToValue(engine)})";

            case nameof(DateTimeExtension.TruncateToQuarter):
                return $"date_trunc('quarter', {mce.Arguments[0].ToValue(engine)})";

            case nameof(DateTimeExtension.TruncateToMonth):
                return $"date_trunc('month', {mce.Arguments[0].ToValue(engine)})";

            case nameof(DateTimeExtension.TruncateToDay):
                return $"date_trunc('day', {mce.Arguments[0].ToValue(engine)})";

            case nameof(DateTimeExtension.TruncateToHour):
                return $"date_trunc('hour',{mce.Arguments[0].ToValue(engine)})";

            case nameof(DateTimeExtension.TruncateToMinute):
                return $"date_trunc('minute', {mce.Arguments[0].ToValue(engine)})";

            case nameof(DateTimeExtension.TruncateToSecond):
                return $"date_trunc('second', {mce.Arguments[0].ToValue(engine)})";

            case nameof(DateTimeExtension.ToMonthEndDate):
                return $"date_trunc('month', {mce.Arguments[0].ToValue(engine)}) + interval '1 month - 1 day'";

            default:
                throw new ArgumentException($"Unsupported method call: {mce.Method.Name}");
        }

        throw new ArgumentException("Invalid argument type for SQL command processing.");
    }

    private static string CreateSqlCommand(this MethodCallExpression mce, BuilderEngine engine)
    {
        switch (mce.Method.Name)
        {
            case nameof(Sql.DateTruncateToYear):
                return $"date_trunc('year', {mce.Arguments[0].ToValue(engine)})";

            case nameof(Sql.DateTruncateToQuarter):
                return $"date_trunc('quarter', {mce.Arguments[0].ToValue(engine)})";

            case nameof(Sql.DateTruncToMonth):
                return $"date_trunc('month', {mce.Arguments[0].ToValue(engine)})";

            case nameof(Sql.DateTruncateToDay):
                return $"date_trunc('day', {mce.Arguments[0].ToValue(engine)})";

            case nameof(Sql.DateTruncateToHour):
                return $"date_trunc('hour', {mce.Arguments[0].ToValue(engine)})";

            case nameof(Sql.DateTruncateToMinute):
                return $"date_trunc('minute', {mce.Arguments[0].ToValue(engine)})";

            case nameof(Sql.DateTruncateToSecond):
                return $"date_trunc('second', {mce.Arguments[0].ToValue(engine)})";

            case nameof(Sql.Raw):
                if (mce.Arguments.First() is ConstantExpression argRaw)
                {
                    return argRaw.Value?.ToString() ?? throw new ArgumentException("Raw SQL argument is null.");
                }
                break;

            case nameof(Sql.RowNumber):
                return Aggregate(mce, engine, "row_number");


            case nameof(Sql.Exists):
            case nameof(Sql.NotExists):
                return ToExistsClause(mce, engine);

            case nameof(Sql.Sum):
                return Aggregate(mce, engine, "sum");

            case nameof(Sql.Count):
                return Aggregate(mce, engine, "count");

            case nameof(Sql.Min):
                return Aggregate(mce, engine, "min");
            case nameof(Sql.Max):
                return Aggregate(mce, engine, "max");
            case nameof(Sql.Average):
                return Aggregate(mce, engine, "avg");

            default:
                throw new ArgumentException($"Unsupported method call: {mce.Method.Name}");
        }

        throw new ArgumentException("Invalid argument type for SQL command processing.");
    }

    private static string Aggregate(MethodCallExpression mce, BuilderEngine engine, string aggregateFunction)
    {
#if DEBUG
        // Analyze the expression tree for debugging purposes
        var analyze = ExpressionReader.Analyze(mce);
#endif

        // Extract the main aggregate function
        string value;
        if (aggregateFunction.IsEqualNoCase("count"))
        {
            value = "count(*)";
        }
        else if (aggregateFunction.IsEqualNoCase("row_number"))
        {
            value = "row_number()";
        }
        else
        {
            value = ExtractFunction(mce, engine, aggregateFunction, mce.Arguments[0]);
        }

        // Determine the argument indices for partition and order
        int partitionArgumentIndex = (aggregateFunction.IsEqualNoCase("count") || aggregateFunction.IsEqualNoCase("row_number")) ? 0 : 1;
        int orderArgumentIndex = partitionArgumentIndex + 1;

        // Extract the partition and order clauses
        // The arguments can be:
        // - Main function argument, partition, order
        // - Partition, order
        // There are no functions that only have a partition or only have an order.
        string partitionby = mce.Arguments.Count <= partitionArgumentIndex ? string.Empty : ExtractPartition(mce, engine, mce.Arguments[partitionArgumentIndex]);
        string orderby = mce.Arguments.Count <= orderArgumentIndex ? string.Empty : ExtractOrder(mce, engine, mce.Arguments[orderArgumentIndex]);

        // Construct the final SQL function string with the over clause
        if (!string.IsNullOrEmpty(partitionby) && !string.IsNullOrEmpty(orderby))
        {
            value += $" over({partitionby} {orderby})";
        }
        else if (!string.IsNullOrEmpty(partitionby))
        {
            value += $" over({partitionby})";
        }
        else if (!string.IsNullOrEmpty(orderby))
        {
            value += $" over({orderby})";
        }

        return value;
    }

    private static string ExtractFunction(MethodCallExpression mce
        , BuilderEngine engine
        , string functionName
        , Expression? argument)
    {
        if (argument == null) throw new NotSupportedException();

        var ue = (UnaryExpression)argument;
        var expression = (LambdaExpression)ue.Operand;

        if (expression.Body is BinaryExpression be)
        {
            var value = be.ToValue(engine);
            return $"{functionName}({value})";
        }
        if (expression.Body is MemberExpression me)
        {
            var value = me.ToValue(engine);
            return $"{functionName}({value})";
        }

        throw new NotSupportedException();
    }

    private static string ExtractPartition(MethodCallExpression mce, BuilderEngine engine, Expression argument)
    {
#if DEBUG
        // Analyze the expression tree for debugging purposes
        var analyze = ExpressionReader.Analyze(argument);
#endif

        var functionName = "partition by";

        var ue = (UnaryExpression)argument;
        var expression = (LambdaExpression)ue.Operand;

        if (expression.Body is ConstantExpression) return string.Empty;

        if (expression.Body is NewExpression ne && ne.Members != null)
        {
            var value = string.Join(",", ne.Arguments.Select(x => x.ToValue(engine)));
            return $"{functionName} {value}";
        }

        throw new NotSupportedException();
    }

    private static string ExtractOrder(MethodCallExpression mce, BuilderEngine engine, Expression argument)
    {
        var functionName = "order by";

        var ue = (UnaryExpression)argument;
        var expression = (LambdaExpression)ue.Operand;

        if (expression.Body is ConstantExpression) return string.Empty;

        if (expression.Body is NewExpression ne && ne.Members != null)
        {
            var cnt = ne.Members.Count();
            var args = new List<string>() { Capacity = cnt };

            // If an alias is specified, it is determined to be in "descending order".
            for (var i = 0; i < cnt; i++)
            {
                var alias = ne.Members[i].Name;
                var val = ne.Arguments[i].ToValue(engine);
                if (ValueParser.Parse(val).GetDefaultName() == alias)
                {
                    args.Add(val);
                }
                else
                {
                    args.Add($"{val} desc");
                }
            }
            var value = string.Join(",", args);
            return $"{functionName} {value}";
        }

        throw new NotSupportedException();
    }

    private static string ToExistsClause(MethodCallExpression mce, BuilderEngine engine)
    {
        var ue = (UnaryExpression)mce.Arguments[1];
        var expression = (LambdaExpression)ue.Operand;
        var tp = expression.Parameters[0].Type;

        var clause = TableDefinitionClauseFactory.Create(tp);

        var fsql = new FluentSelectQuery();
        var (f, x) = fsql.From(clause).As(expression.Parameters[0].Name!);

        var prmManager = new ParameterManager(fsql.GetParameters(), fsql.AddParameter);
        var eng = new BuilderEngine(engine.SqlDialect, prmManager);

        var value = expression.Body.ToValue(eng);
        fsql.Where(value);

        if (mce.Method.Name == nameof(Sql.Exists))
        {
            return fsql.ToExists().ToText();
        }
        else
        {
            return fsql.ToNotExists().ToText();
        }
    }

    private static string ToStringValue(this MethodCallExpression mce, BuilderEngine engine)
    {
        if (mce.Object != null)
        {
            var value = mce.Object.ToValue(engine);
            if (mce.Arguments.Count == 0)
            {
                if (mce.Method.Name == nameof(String.TrimStart))
                {
                    return $"ltrim({value})";
                }
                if (mce.Method.Name == nameof(String.Trim))
                {
                    return $"trim({value})";
                }
                if (mce.Method.Name == nameof(String.TrimEnd))
                {
                    return $"rtrim({value})";
                }
                if (mce.Method.Name == nameof(String.ToString))
                {
                    return engine.SqlDialect.GetCastStatement(value, typeof(string));
                }
            }

            if (mce.Arguments.Count == 1)
            {
                if (mce.Method.Name == nameof(String.ToString))
                {
                    var mng = new StringFormatParameterManager(engine.Manager);
                    var eng = new BuilderEngine(engine.SqlDialect, mng);

                    var format = mce.Arguments[0].ToValue(eng);
                    return $"to_char({value}, {format})";
                }

                var arg = mce.Arguments[0].ToValue(engine);
                if (mce.Method.Name == nameof(String.StartsWith))
                {
                    return $"{value} like {arg} || '%'";
                }
                if (mce.Method.Name == nameof(String.Contains))
                {
                    return $"{value} like '%' || {arg} || '%'";
                }
                if (mce.Method.Name == nameof(String.EndsWith))
                {
                    return $"{value} like {arg} || '%'";
                }
            }

            throw new NotSupportedException($"Object:{mce.Object.Type.FullName}, Method:{mce.Method.Name}, Arguments:{mce.Arguments.Count}, Type:{mce.Type}");
        }

        throw new NotSupportedException($"Object:NULL, Method:{mce.Method.Name}, Arguments:{mce.Arguments.Count}, Type:{mce.Type}");
    }

    private static string ToBoolValue(this MethodCallExpression mce, BuilderEngine engine)
    {
        if (mce.Object != null)
        {
            if (mce.Arguments.Count == 1)
            {
                if (mce.Method.DeclaringType == typeof(string))
                {
                    var value = mce.Object.ToValue(engine);

                    var arg = mce.Arguments[0].ToValue(engine);
                    if (mce.Method.Name == nameof(String.StartsWith))
                    {
                        return $"{value} like {arg} || '%'";
                    }
                    if (mce.Method.Name == nameof(String.Contains))
                    {
                        return $"{value} like '%' || {arg} || '%'";
                    }
                    if (mce.Method.Name == nameof(String.EndsWith))
                    {
                        return $"{value} like {arg} || '%'";
                    }
                }
                if (IsGenericList(mce.Object.Type))
                {
                    //The IN clause itself is not suitable for parameter queries, so the collection will be forcibly expanded.
                    //If you want to parameterize it, use the ANY function, etc.

                    var prmManger = new CollectionParameterManager(engine.Manager);
                    var eng = new BuilderEngine(engine.SqlDialect, prmManger);

                    _ = mce.Object.ToValue(eng);

                    var left = mce.Arguments[0].ToValue(engine);
                    if (mce.Method.Name == nameof(String.Contains))
                    {
                        return $"{left} in ({prmManger.GetCollectionValue})";
                    }
                }
            }

            throw new NotSupportedException($"Object:{mce.Object.Type.FullName}, Method.Type:{mce.Method.DeclaringType}, Method.Method:{mce.Method.Name}, Arguments:{mce.Arguments.Count}, Type:{mce.Type}");
        }
        else
        {
            if (mce.Arguments.Count == 2)
            {
                if (mce.Method.DeclaringType == typeof(Enumerable))
                {
                    if (mce.Method.Name == nameof(Enumerable.Any))
                    {
                        return ToAnyClauseValue(mce, engine);
                    }
                }
            }
            throw new NotSupportedException($"Object:NULL, Method.Type:{mce.Method.DeclaringType}, Method.Name:{mce.Method.Name}, Arguments:{mce.Arguments.Count}, Type:{mce.Type}");
        }
    }

    private static string ToAnyClauseValue(this MethodCallExpression mce, BuilderEngine engine)
    {
        // Format:
        // Array.Any(x => (table.column == x))
        // or
        // Array.Any(x => (x == table.column))

        var lambda = (LambdaExpression)mce.Arguments[1];

        var variableName = lambda.Parameters[0].Name!;

        var arrayParameterName = mce.Arguments[0].ToValue(engine);


        // Hook the parameter name and return in the format any(PARAMETER)
        var prmManager = new ArrayParameterManager(engine, variableName, arrayParameterName);
        var interceptor = new BuilderEngine(engine.SqlDialect, prmManager);

        var body = (BinaryExpression)lambda.Body;
        if (body.NodeType != ExpressionType.Equal) throw new InvalidProgramException();

        // Adjust to make the ANY function appear on the right side
        var left = body.Left.ToValue(interceptor);
        if (prmManager.IsCommandCreated)
        {
            //right = any(left)
            return $"{body.Right.ToValue(interceptor)} = {left}";
        }

        var right = body.Right.ToValue(interceptor);
        if (prmManager.IsCommandCreated)
        {
            //left = any(right)
            return $"{left} = {right}";
        }

        throw new InvalidProgramException();
    }

    private static string ToDateTimeValue(this MethodCallExpression mce, BuilderEngine engine)
    {
        if (mce!.Object != null)
        {
            var value = mce!.Object!.ToValue(engine);

            if (mce.Arguments.Count == 1)
            {
                var arg = mce.Arguments[0].ToValue(engine);

                if (mce.Method.Name == nameof(DateTime.AddYears))
                {
                    return $"{value} + {arg} * interval '1 year'";
                }
                if (mce.Method.Name == nameof(DateTime.AddMonths))
                {
                    return $"{value} + {arg} * interval '1 month'";
                }
                if (mce.Method.Name == nameof(DateTime.AddDays))
                {
                    return $"{value} + {arg} * interval '1 day'";
                }
                if (mce.Method.Name == nameof(DateTime.AddHours))
                {
                    return $"{value} + {arg} * interval '1 hour'";
                }
                if (mce.Method.Name == nameof(DateTime.AddMinutes))
                {
                    return $"{value} + {arg} * interval '1 minute'";
                }
                if (mce.Method.Name == nameof(DateTime.AddSeconds))
                {
                    return $"{value} + {arg} * interval '1 second'";
                }
                if (mce.Method.Name == nameof(DateTime.AddMilliseconds))
                {
                    return $"{value} + {arg} * interval '1 ms'";
                }
            }

            throw new NotSupportedException($"Object:{mce.Object.Type.FullName}, Method:{mce.Method.Name}, Arguments:{mce.Arguments.Count}, Type:{mce.Type}");
        }
        else
        {
            if (mce.Arguments.Count == 1)
            {
                var echo = new BuilderEngine(engine.SqlDialect, new EchoParameterManager());
                return mce.ToValue(echo);
            }

            throw new NotSupportedException($"Object:NULL, Method:{mce.Method.Name}, Arguments:{mce.Arguments.Count}, Type:{mce.Type}");
        }
    }

    private static string RemoveRootBracketOrDefault(string value)
    {
        if (value.StartsWith("("))
        {
            //remove excess parentheses
            if (BracketValueParser.TryParse(value, out var v) && v is BracketValue bv)
            {
                return bv.Inner.ToText();
            }
        }
        return value;
    }


    private static bool IsGenericList(Type? type)
    {
        if (type == null) return false;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
        {
            return true;
        }

        foreach (Type intf in type.GetInterfaces())
        {
            if (intf.IsGenericType && intf.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return true;
            }
        }

        return false;
    }

    private static string AddParameter(Object? value
       , Func<string, object?, string> addParameter)
    {
        if (value == null)
        {
            return "null";
        }

        var tp = value.GetType();

        if (tp == typeof(string))
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return "''";
            }
            else
            {
                return addParameter(string.Empty, value);
            }
        }
        else if (tp == typeof(DateTime))
        {
            return addParameter(string.Empty, value);
        }
        else
        {
            return value.ToString()!;
        }
    }
}
