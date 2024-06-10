using Carbunql.Analysis.Parser;
using Carbunql.Annotations;
using Carbunql.Building;
using Carbunql.Extensions;
using Carbunql.TypeSafe.Extensions;
using Carbunql.Values;
using System.Collections;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class MethodCallExpressionExtension
{
    internal static string ToValue(this MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        // DeclaringType
        if (mce.Method.DeclaringType == typeof(Math))
        {
            return CreateMathCommand(mce, mainConverter, addParameter);
        }
        if (mce.Method.DeclaringType == typeof(Sql))
        {
            return CreateSqlCommand(mce, mainConverter, addParameter);
        }
        if (mce.Method.DeclaringType == typeof(DateTimeExtension))
        {
            return CreateDateTimeExtensionCommand(mce, mainConverter, addParameter);
        }

        // Return Type
        if (mce.Type == typeof(string))
        {
            return ToStringValue(mce, mainConverter, addParameter);
        }
        if (mce.Type == typeof(bool))
        {
            return ToBoolValue(mce, mainConverter, addParameter);
        }
        if (mce.Type == typeof(DateTime))
        {
            return ToDateTimeValue(mce, mainConverter, addParameter);
        }

        throw new NotSupportedException($"Type:{mce.Type}, Method.DeclaringType:{mce.Method.DeclaringType}");
    }

    private static string CreateMathCommand(this MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        var args = mce.Arguments.Select(x => RemoveRootBracketOrDefault(mainConverter(x, addParameter)));

        return mce.Method.Name switch
        {
            nameof(Math.Truncate) => DbmsConfiguration.GetTruncateCommandLogic(args),
            nameof(Math.Floor) => DbmsConfiguration.GetFloorCommandLogic(args),
            nameof(Math.Ceiling) => DbmsConfiguration.GetCeilingCommandLogic(args),
            nameof(Math.Round) => DbmsConfiguration.GetRoundCommandLogic(args),
            _ => throw new NotSupportedException($"The method '{mce.Method.Name}' is not supported.")
        };
    }

    private static string CreateDateTimeExtensionCommand(MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        switch (mce.Method.Name)
        {
            case nameof(DateTimeExtension.TruncateToYear):
                return $"date_trunc('year', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(DateTimeExtension.TruncateToQuarter):
                return $"date_trunc('quarter', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(DateTimeExtension.TruncateToMonth):
                return $"date_trunc('month', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(DateTimeExtension.TruncateToDay):
                return $"date_trunc('day', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(DateTimeExtension.TruncateToHour):
                return $"date_trunc('hour', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(DateTimeExtension.TruncateToMinute):
                return $"date_trunc('minute', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(DateTimeExtension.TruncateToSecond):
                return $"date_trunc('second', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(DateTimeExtension.ToMonthEndDate):
                return $"date_trunc('month', {mainConverter(mce.Arguments[0], addParameter)}) + interval '1 month - 1 day'";

            default:
                throw new ArgumentException($"Unsupported method call: {mce.Method.Name}");
        }

        throw new ArgumentException("Invalid argument type for SQL command processing.");
    }

    private static string CreateSqlCommand(this MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        switch (mce.Method.Name)
        {
            case nameof(Sql.DateTruncateToYear):
                return $"date_trunc('year', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncateToQuarter):
                return $"date_trunc('quarter', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncToMonth):
                return $"date_trunc('month', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncateToDay):
                return $"date_trunc('day', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncateToHour):
                return $"date_trunc('hour', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncateToMinute):
                return $"date_trunc('minute', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncateToSecond):
                return $"date_trunc('second', {mainConverter(mce.Arguments[0], addParameter)})";

            case nameof(Sql.Raw):
                if (mce.Arguments.First() is ConstantExpression argRaw)
                {
                    return argRaw.Value?.ToString() ?? throw new ArgumentException("Raw SQL argument is null.");
                }
                break;

            case nameof(Sql.RowNumber):
                return Aggregate(mce, mainConverter, addParameter, "row_number");


            case nameof(Sql.Exists):
            case nameof(Sql.NotExists):
                return ToExistsClause(mce);

            case nameof(Sql.Sum):
                return Aggregate(mce, mainConverter, addParameter, "sum");

            case nameof(Sql.Count):
                return Aggregate(mce, mainConverter, addParameter, "count");

            case nameof(Sql.Min):
                return Aggregate(mce, mainConverter, addParameter, "min");
            case nameof(Sql.Max):
                return Aggregate(mce, mainConverter, addParameter, "max");
            case nameof(Sql.Average):
                return Aggregate(mce, mainConverter, addParameter, "avg");

            default:
                throw new ArgumentException($"Unsupported method call: {mce.Method.Name}");
        }

        throw new ArgumentException("Invalid argument type for SQL command processing.");
    }

    private static string Aggregate(MethodCallExpression mce
    , Func<Expression, Func<string, object?, string>, string> mainConverter
    , Func<string, object?, string> addParameter
    , string aggregateFunction)
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
            value = ExtractFunction(mce, mainConverter, addParameter, aggregateFunction, mce.Arguments[0]);
        }

        // Determine the argument indices for partition and order
        int partitionArgumentIndex = (aggregateFunction.IsEqualNoCase("count") || aggregateFunction.IsEqualNoCase("row_number")) ? 0 : 1;
        int orderArgumentIndex = partitionArgumentIndex + 1;

        // Extract the partition and order clauses
        // The arguments can be:
        // - Main function argument, partition, order
        // - Partition, order
        // There are no functions that only have a partition or only have an order.
        string partitionby = mce.Arguments.Count <= partitionArgumentIndex ? string.Empty : ExtractPartition(mce, mainConverter, addParameter, mce.Arguments[partitionArgumentIndex]);
        string orderby = mce.Arguments.Count <= orderArgumentIndex ? string.Empty : ExtractOrder(mce, mainConverter, addParameter, mce.Arguments[orderArgumentIndex]);

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
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter
        , string functionName
        , Expression? argument)
    {
        if (argument == null) throw new NotSupportedException();

        var ue = (UnaryExpression)argument;
        var expression = (LambdaExpression)ue.Operand;

        if (expression.Body is BinaryExpression be)
        {
            var value = be.ToValue(mainConverter, addParameter);
            return $"{functionName}({value})";
        }
        if (expression.Body is MemberExpression me)
        {
            var value = me.ToValue(mainConverter, addParameter);
            return $"{functionName}({value})";
        }

        throw new NotSupportedException();
    }

    private static string ExtractPartition(MethodCallExpression mce
     , Func<Expression, Func<string, object?, string>, string> mainConverter
     , Func<string, object?, string> addParameter
     , Expression argument)
    {
        var functionName = "partition by";

        var ue = (UnaryExpression)argument;
        var expression = (LambdaExpression)ue.Operand;

        if (expression.Body is ConstantExpression) return string.Empty;

        if (expression.Body is NewExpression ne && ne.Members != null)
        {
            var value = string.Join(",", ne.Arguments.Select(x => x.ToValue(addParameter)));
            return $"{functionName} {value}";
        }

        throw new NotSupportedException();
    }

    private static string ExtractOrder(MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter
        , Expression argument)
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
                var val = ne.Arguments[i].ToValue(addParameter);
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

    private static string ToExistsClause(MethodCallExpression mce)
    {
        var ue = (UnaryExpression)mce.Arguments[1];
        var expression = (LambdaExpression)ue.Operand;
        var tp = expression.Parameters[0].Type;

        var clause = TableDefinitionClauseFactory.Create(tp);

        var fsql = new FluentSelectQuery();
        var (f, x) = fsql.From(clause).As(expression.Parameters[0].Name!);
        var prmManager = new ParameterManager(fsql.GetParameters(), fsql.AddParameter);
        var value = expression.Body.ToValue(prmManager.AddParameter);
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

    private static string ToStringValue(this MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        if (mce.Object != null)
        {
            var value = mainConverter(mce.Object, addParameter);
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
                    return FluentSelectQuery.CreateCastStatement(value, typeof(string));
                }
            }

            if (mce.Arguments.Count == 1)
            {
                if (mce.Method.Name == nameof(String.ToString))
                {
                    Func<string, object?, string> typeCaster = (key, value) =>
                    {
                        var v = ConverToDbDateFormat(value!.ToString()!);
                        return addParameter(key, v);
                    };
                    var typedArg = mainConverter(mce.Arguments[0], typeCaster);
                    return $"to_char({value}, {typedArg})";
                }

                var arg = mainConverter(mce.Arguments[0], addParameter);
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

    private static string ToBoolValue(this MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        if (mce.Object != null)
        {
            if (mce.Arguments.Count == 1)
            {
                if (mce.Method.DeclaringType == typeof(string))
                {
                    var value = mainConverter(mce.Object, addParameter);

                    var arg = mainConverter(mce.Arguments[0], addParameter);
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
                    var args = new List<string>();
                    Func<string, object?, string> argumentsDecoder = (_, collection) =>
                    {
                        if (collection != null && IsGenericList(collection.GetType()))
                        {
                            foreach (var item in (IEnumerable)collection)
                            {
                                args.Add(AddParameter(item, addParameter));
                            }
                        }
                        return string.Empty;
                    };

                    _ = mainConverter(mce.Object, argumentsDecoder);

                    var left = mainConverter(mce.Arguments[0], addParameter);
                    if (mce.Method.Name == nameof(String.Contains))
                    {
                        return $"{left} in({string.Join(",", args)})";
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
                        return ToAnyClauseValue(mce, mainConverter, addParameter);
                    }
                }
            }
            throw new NotSupportedException($"Object:NULL, Method.Type:{mce.Method.DeclaringType}, Method.Name:{mce.Method.Name}, Arguments:{mce.Arguments.Count}, Type:{mce.Type}");
        }
    }

    private static string ToAnyClauseValue(this MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        // Format:
        // Array.Any(x => (table.column == x))
        // or
        // Array.Any(x => (x == table.column))

        var lambda = (LambdaExpression)mce.Arguments[1];
        var variableName = lambda.Parameters[0].Name!;

        var arrayParameterName = mainConverter(mce.Arguments[0], addParameter);

        // Hook the parameter name and return in the format any(PARAMETER)
        var hasAnyCommand = false;
        Func<string, object?, string> interceptor = (_, x) =>
        {
            if (variableName.Equals(x))
            {
                hasAnyCommand = true;
                return $"any({arrayParameterName})";
            }
            throw new InvalidProgramException();
        };

        var body = (BinaryExpression)lambda.Body;
        if (body.NodeType != ExpressionType.Equal) throw new InvalidProgramException();

        // Adjust to make the ANY function appear on the right side
        var left = mainConverter(body.Left, interceptor);
        if (hasAnyCommand)
        {
            return $"{mainConverter(body.Right, interceptor)} = {left}";
        }

        var right = mainConverter(body.Right, interceptor);
        if (hasAnyCommand)
        {
            return $"{left} = {right}";
        }

        throw new InvalidProgramException();
    }

    private static string ToDateTimeValue(this MethodCallExpression mce
        , Func<Expression, Func<string, object?, string>, string> mainConverter
        , Func<string, object?, string> addParameter)
    {
        if (mce!.Object != null)
        {
            var value = mainConverter(mce!.Object!, addParameter);

            if (mce.Arguments.Count == 1)
            {
                var arg = mainConverter(mce.Arguments[0], addParameter);

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
                Func<string, object?, string> echo = (_, x) => x!.ToString()!;
                return mce.ToValue(mainConverter, echo);
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

    private static string ConverToDbDateFormat(string csharpFormat)
    {
        var replacements = new Dictionary<string, string>
        {
            {"yyyy", "YYYY"},
            {"MM", "MM"},
            {"dd", "DD"},
            {"HH", "HH24"},
            {"mm", "MI"},
            {"ss", "SS"},
            {"ffffff", "US"},
            {"fff", "MS"}
        };

        string dbformat = csharpFormat;

        foreach (var pair in replacements)
        {
            dbformat = dbformat.Replace(pair.Key, pair.Value);
        }

        return dbformat;
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
