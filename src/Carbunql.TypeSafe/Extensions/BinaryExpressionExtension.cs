using Carbunql.Analysis.Parser;
using Carbunql.Extensions;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class BinaryExpressionExtension
{
    internal static string ToValue(this BinaryExpression be, BuilderEngine engine)
    {
        var left = be.Left.ToValue(engine);
        var right = be.Right.ToValue(engine);
        return ToValue(be.NodeType, left, right, engine);
    }

    private static string ToValue(ExpressionType nodeType, string left, string right, BuilderEngine engine)
    {
        var opPrecedence = GetPrecedenceFromExpressionType(nodeType);

        var leftValue = ValueParser.Parse(left);
        var rightValue = ValueParser.Parse(right);

        // Enclose expressions in parentheses based on operator precedence or specific conditions
        if (nodeType == ExpressionType.OrElse)
        {
            // If an AND condition exists, enclose the whole in parentheses.
            if (leftValue.GetOperators().Where(x => x.IsEqualNoCase("and")).Any())
            {
                left = $"({left})";
            }
            if (rightValue.GetOperators().Where(x => x.IsEqualNoCase("and")).Any())
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
            ExpressionType.Coalesce => engine.SqlDialect.GetCoalesceCommand(left, right),
            ExpressionType.Add => $"{left} + {right}",
            ExpressionType.Subtract => $"{left} - {right}",
            ExpressionType.Multiply => $"{left} * {right}",
            ExpressionType.Divide => $"{left} / {right}",
            ExpressionType.Modulo => engine.SqlDialect.GetModuloCommand(left, right),
            ExpressionType.Equal => $"{left} = {right}",
            ExpressionType.NotEqual => $"{left} <> {right}",
            ExpressionType.GreaterThan => $"{left} > {right}",
            ExpressionType.GreaterThanOrEqual => $"{left} >= {right}",
            ExpressionType.LessThan => $"{left} < {right}",
            ExpressionType.LessThanOrEqual => $"{left} <= {right}",
            ExpressionType.AndAlso => $"{left} and {right}",
            ExpressionType.OrElse => $"({left} or {right})",
            _ => throw new NotSupportedException($"Unsupported expression type: {nodeType}")
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

}
