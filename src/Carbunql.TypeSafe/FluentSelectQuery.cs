using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.TypeSafe.Extensions;
using Carbunql.Values;
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

        var prms = this.GetParameters().ToList();
        var parameterCount = prms.Count();
        Func<object?, string> addParameter = (obj) =>
        {
            if (obj != null)
            {
                var q = prms.Where(x => x.Value != null && x.Value.Equals(obj));
                if (q.Any())
                {
                    return q.First().ParameterName;
                }
            }

            var pname = $"{DbmsConfiguration.PlaceholderIdentifier}p{parameterCount}";
            parameterCount++;
            AddParameter(pname, obj);

            if (obj != null)
            {
                prms.Add(new QueryParameter(pname, obj));
            }

            return pname;
        };

        if (body.Members != null)
        {
            var cnt = body.Members.Count();
            for (var i = 0; i < cnt; i++)
            {
                var alias = body.Members[i].Name;
                var value = ToValue(body.Arguments[i], addParameter);
                this.Select(RemoveRootBracketOrDefault(value)).As(alias);
            }
        }

        return this;
    }

    public FluentSelectQuery Where(Expression<Func<bool>> expression)
    {
#if DEBUG
        var analyzed = ExpressionReader.Analyze(expression);
#endif

        var prms = this.GetParameters().ToList();
        var parameterCount = prms.Count();
        Func<object?, string> addParameter = (obj) =>
        {
            if (obj != null)
            {
                var q = prms.Where(x => x.Value != null && x.Value.Equals(obj));
                if (q.Any())
                {
                    return q.First().ParameterName;
                }
            }

            var pname = $"{DbmsConfiguration.PlaceholderIdentifier}p{parameterCount}";
            parameterCount++;
            AddParameter(pname, obj);

            if (obj != null)
            {
                prms.Add(new QueryParameter(pname, obj));
            }

            return pname;
        };

        if (expression.Body is MethodCallExpression mce)
        {
            this.Where(ToValue(mce, addParameter));
            return this;
        }
        else if (expression.Body is BinaryExpression be)
        {
            var value = ToValue(be, addParameter);
            if (be.NodeType == ExpressionType.OrElse)
            {
                this.Where($"({value})");
            }
            else
            {
                this.Where(value);
            }
            return this;
        }

        throw new Exception();
    }

    private string ToValue(Expression exp, Func<object?, string> addParameter)
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
            return mem.ToValue(ToValue, addParameter);
        }
        else if (exp is ConstantExpression ce)
        {
            // static value
            return fn(ce.Value, ce.Type);
        }
        else if (exp is NewExpression ne)
        {
            // ex. new Datetime
            return fn(ne.CompileAndInvoke(), ne.Type);
        }
        else if (exp is BinaryExpression be)
        {
            var left = ToValue(be.Left, addParameter);
            var right = ToValue(be.Right, addParameter);
            return ToValue(be.NodeType, left, right);
        }
        else if (exp is UnaryExpression ue)
        {
            if (ue.NodeType == ExpressionType.Convert)
            {
                return CreateCastStatement(ue, addParameter);
            }
            throw new InvalidProgramException(exp.ToString());
        }
        else if (exp is MethodCallExpression mce)
        {
            return mce.ToValue(ToValue, addParameter);
        }
        else if (exp is ConditionalExpression cnd)
        {
            return CreateCaseStatement(cnd, addParameter);
        }

        throw new InvalidProgramException(exp.ToString());
    }

    private string ToValue(ExpressionType nodeType, string left, string right)
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

    private string CreateCastStatement(UnaryExpression ue, Func<object?, string> addParameter)
    {
        var value = ToValue(ue.Operand, addParameter);
        return CreateCastStatement(value, ue.Type);
    }

    internal static string CreateCastStatement(string value, Type type)
    {
        var dbtype = DbmsConfiguration.ToDbType(type);
        return $"cast({value} as {dbtype})";
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

    internal static string ConverToDbDateFormat(string csharpFormat)
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
}
