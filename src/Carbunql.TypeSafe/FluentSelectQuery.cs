using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Data.Common;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

public class FluentSelectQuery : SelectQuery
{
    public FluentSelectQuery Select<T>(Expression<Func<T>> expression) where T : class
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif

        var body = (NewExpression)expression.Body;

        var parameterDictionary = new Dictionary<object, string>();
        var parameterCount = this.GetParameters().Count();
        Func<object?, string> addParameter = (obj) =>
        {
            if (obj != null && parameterDictionary.ContainsKey(obj))
            {
                return parameterDictionary[obj];
            }

            var pname = $"{DbmsConfiguration.PlaceholderIdentifier}p{parameterCount}";
            parameterCount++;
            AddParameter(pname, obj);

            if (obj != null)
            {
                parameterDictionary[obj] = pname;
            }

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
                    this.Select(RemoveRootBracketOrDefault(value)).As(alias);
                }
            }
        }

        return this;
    }

    public FluentSelectQuery Where(Expression<Func<bool>> expression)
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif

        var body = (BinaryExpression)expression.Body;

        var parameterCount = this.GetParameters().Count();
        Func<object?, string> addParameter = (obj) =>
        {
            var pname = $"{DbmsConfiguration.PlaceholderIdentifier}p{parameterCount}";
            parameterCount++;
            AddParameter(pname, obj);
            return pname;
        };

        var value = ToValue(body, addParameter);
        if (body.NodeType == ExpressionType.OrElse)
        {
            this.Where($"({value})");
        }
        else
        {
            this.Where(value);
        }
        return this;
    }

    private string RemoveRootBracketOrDefault(string value)
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

    private string ToValue(Expression exp, Func<object?, string> addParameter)
    {
        if (TryToValue(exp, addParameter, out var value))
        {
            return value;
        }
        throw new Exception();
    }

    private bool TryToValue(Expression exp, Func<object?, string> addParameter, out string value)
    {
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
                // ex. Sql.Now, Sql.CurrentTimestamp
                value = CreateSqlCommand(mem);
                return true;
            }
            if (mem.Expression is MemberExpression && typeof(ITableRowDefinition).IsAssignableFrom(tp))
            {
                //column
                var table = ((MemberExpression)mem.Expression).Member.Name;
                var column = mem.Member.Name;
                value = $"{table}.{column}";
                return true;
            }
            if (mem.Expression is ConstantExpression ce)
            {
                //variable
                value = addParameter(mem.CompileAndInvoke());
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
                value = GetValue(be.NodeType, left, right);
                return true;
            }
        }
        else if (exp is UnaryExpression ue)
        {
            if (ue.NodeType == ExpressionType.Convert)
            {
                value = CreateCastStatement(ue, addParameter);
                return true;
            }
            throw new InvalidProgramException(exp.ToString());
        }
        else if (exp is MethodCallExpression mce)
        {
            if (mce.Method.DeclaringType == typeof(Math))
            {
                // Math methods like Math.Truncate, Math.Round
                value = CreateMathCommand(mce, addParameter);
                return true;
            }
            if (mce.Method.DeclaringType == typeof(Sql))
            {
                // Reserved SQL command
                value = CreateSqlCommand(mce, addParameter);
                return true;
            }

            value = ParseSqlCommand(exp, addParameter);
            return true;
        }
        else if (exp is ConditionalExpression cnd)
        {
            value = CreateCaseStatement(cnd, addParameter);
            return true;
        }

        throw new InvalidOperationException(exp.ToString());
    }

    private string ParseSqlCommand(Expression expression, Func<object?, string> addParameter)
    {
        var value = string.Empty;
        if (expression is MemberExpression memberExpression)
        {
            if (memberExpression.Member.DeclaringType == typeof(Sql))
            {
                return CreateSqlCommand(memberExpression);
            }
            return ToValue(memberExpression, addParameter);
        }

        // 現在の式が MethodCallExpression かどうかを確認し、子を探索
        if (expression is MethodCallExpression mce && mce.Object != null)
        {
            value = ParseSqlCommand(mce.Object, addParameter);

            var arg = ToValue(mce.Arguments[0], addParameter);
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
            throw new Exception();
        }

        throw new Exception();
    }

    private static string GetValue(ExpressionType nodeType, string left, string right)
    {
        var opPrecedence = GetPrecedenceFromExpressionType(nodeType);

        var leftValue = ValueParser.Parse(left);
        var rightValue = ValueParser.Parse(right);

        // Enclose expressions in parentheses based on operator precedence or specific conditions
        if (nodeType == ExpressionType.OrElse)
        {
            if (leftValue.GetOperators().Any())
            {
                left = $"({left})";
            }
            if (rightValue.GetOperators().Any())
            {
                right = $"({right})";
            }
        }
        else if (opPrecedence == 2)
        {
            if (leftValue.GetOperators().Any(x => GetOperatorPrecedence(x) < opPrecedence))
            {
                left = $"({left})";
            }
            if (rightValue.GetOperators().Any(x => GetOperatorPrecedence(x) < opPrecedence))
            {
                right = $"({right})";
            }
        }

        // Return the formatted expression based on the operation type
        return nodeType switch
        {
            ExpressionType.Coalesce => $"{DbmsConfiguration.CoalesceFunctionName}({left}, {right})",
            ExpressionType.Add => $"{left} + {right}",
            ExpressionType.Subtract => $"{left} - {right}",
            ExpressionType.Multiply => $"{left} * {right}",
            ExpressionType.Divide => $"{left} / {right}",
            ExpressionType.Modulo => $"{DbmsConfiguration.GetModuloCommandLogic(left, right)}",
            ExpressionType.Equal => $"{left} = {right}",
            ExpressionType.NotEqual => $"{left} <> {right}",
            ExpressionType.GreaterThan => $"{left} > {right}",
            ExpressionType.GreaterThanOrEqual => $"{left} >= {right}",
            ExpressionType.LessThan => $"{left} < {right}",
            ExpressionType.LessThanOrEqual => $"{left} <= {right}",
            ExpressionType.AndAlso => $"{left} and {right}",
            ExpressionType.OrElse => $"{left} or {right}",
            _ => throw new NotSupportedException($"Unsupported expression type: {nodeType}")
        };
    }

    private static int GetOperatorPrecedence(string operatorText)
    {
        return operatorText switch
        {
            "+" => 1,
            "-" => 1,
            "*" => 2,
            "/" => 2,
            _ => 0,
        };
    }

    private static int GetPrecedenceFromExpressionType(ExpressionType nodeType)
    {
        var operatorText = nodeType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            _ => string.Empty,
        };
        return GetOperatorPrecedence(operatorText);
    }

    private string CreateMathCommand(MethodCallExpression mce, Func<object?, string> addParameter)
    {
        var args = mce.Arguments.Select(x => RemoveRootBracketOrDefault(ToValue(x, addParameter)));

        return mce.Method.Name switch
        {
            nameof(Math.Truncate) => DbmsConfiguration.GetTruncateCommandLogic(args),
            nameof(Math.Floor) => DbmsConfiguration.GetFloorCommandLogic(args),
            nameof(Math.Ceiling) => DbmsConfiguration.GetCeilingCommandLogic(args),
            nameof(Math.Round) => DbmsConfiguration.GetRoundCommandLogic(args),
            _ => throw new NotSupportedException($"The method '{mce.Method.Name}' is not supported.")
        };
    }

    private string CreateCastStatement(UnaryExpression ue, Func<object?, string> addParameter)
    {
        var value = ToValue(ue.Operand, addParameter);
        return CreateCastStatement(value, ue.Type);
    }

    private string CreateCastStatement(string value, Type type)
    {
        var dbtype = DbmsConfiguration.ToDbType(type);
        return $"cast({value} as {dbtype})";
    }

    private string CreateSqlCommand(MemberExpression mem)
    {
        return mem.Member.Name switch
        {
            nameof(Sql.Now) => CreateCastStatement(DbmsConfiguration.GetNowCommandLogic(), typeof(DateTime)),
            nameof(Sql.CurrentTimestamp) => DbmsConfiguration.GetCurrentTimestampCommandLogic(),
            _ => throw new NotSupportedException($"The member '{mem.Member.Name}' is not supported.")
        };
    }

    private string CreateSqlCommand(MethodCallExpression mce, Func<object?, string> addParameter)
    {
        switch (mce.Method.Name)
        {
            case nameof(Sql.DateTruncYear):
                return $"date_trunc('year', {ToValue(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncQuarter):
                return $"date_trunc('quarter', {ToValue(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncMonth):
                return $"date_trunc('month', {ToValue(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncDay):
                return $"date_trunc('day', {ToValue(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncHour):
                return $"date_trunc('hour', {ToValue(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncMinute):
                return $"date_trunc('minute', {ToValue(mce.Arguments[0], addParameter)})";

            case nameof(Sql.DateTruncSecond):
                return $"date_trunc('second', {ToValue(mce.Arguments[0], addParameter)})";

            case nameof(Sql.Raw):
                if (mce.Arguments.First() is ConstantExpression argRaw)
                {
                    return argRaw.Value?.ToString() ?? throw new ArgumentException("Raw SQL argument is null.");
                }
                break;

            case nameof(Sql.RowNumber):
                if (mce.Arguments.Count == 0)
                {
                    try
                    {
                        return DbmsConfiguration.GetRowNumberCommandLogic();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to get RowNumber command logic.", ex);
                    }
                }
                if (mce.Arguments.Count == 2)
                {
                    try
                    {
                        var argList = mce.Arguments.ToList();
                        if (argList[0] is NewExpression arg1st && argList[1] is NewExpression arg2nd)
                        {
                            var arg1stText = string.Join(",", arg1st.Arguments.Select(x => ToValue(x, addParameter)));
                            var arg2ndText = string.Join(",", arg2nd.Arguments.Select(x => ToValue(x, addParameter)));

                            return DbmsConfiguration.GetRowNumberPartitionByOrderByCommandLogic(arg1stText, arg2ndText);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to process RowNumber with parameters.", ex);
                    }
                }
                throw new ArgumentException("Invalid arguments count for RowNumber.");

            case nameof(Sql.RowNumberOrderbyBy):
                if (mce.Arguments.First() is NewExpression argOrderbyBy)
                {
                    var argOrderbyByText = string.Join(",", argOrderbyBy.Arguments.Select(x => ToValue(x, addParameter)));
                    return DbmsConfiguration.GetRowNumberOrderByCommandLogic(argOrderbyByText);
                }
                break;

            case nameof(Sql.RowNumberPartitionBy):
                if (mce.Arguments.First() is NewExpression argPartitionBy)
                {
                    var argPartitionByText = string.Join(",", argPartitionBy.Arguments.Select(x => ToValue(x, addParameter)));
                    return DbmsConfiguration.GetRowNumberPartitionByCommandLogic(argPartitionByText);
                }
                break;

            default:
                throw new ArgumentException($"Unsupported method call: {mce.Method.Name}");
        }

        throw new ArgumentException("Invalid argument type for SQL command processing.");
    }

    private string CreateCaseStatement(ConditionalExpression cnd, Func<object?, string> addParameter)
    {
        var test = ToValue(cnd.Test, addParameter);
        var ifTrue = ToValue(cnd.IfTrue, addParameter);
        var ifFalse = ToValue(cnd.IfFalse, addParameter);

        if (string.IsNullOrEmpty(ifFalse))
        {
            throw new ArgumentException("The IfFalse expression cannot be null or empty.", nameof(cnd.IfFalse));
        }

        // When case statements are nested, check if there is an alternative in the when clause
        if (ifFalse.TrimStart().StartsWith("case ", StringComparison.OrdinalIgnoreCase))
        {
            var caseExpression = CaseExpressionParser.Parse(ifFalse);
            if (caseExpression.CaseCondition is null)
            {
                // Replace with when clause
                var we = WhenExpressionParser.Parse($"when {test} then {ifTrue}");
                caseExpression.WhenExpressions.Insert(0, we);
                return caseExpression.ToText();
            }
            else
            {
                return $"case when {test} then {ifTrue} else {ifFalse} end";
            }
        }
        else
        {
            return $"case when {test} then {ifTrue} else {ifFalse} end";
        }
    }
}
