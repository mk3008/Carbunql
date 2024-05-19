using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class UnaryExpressionExtension
{
    internal static string ToValue(this UnaryExpression ue
        , Func<Expression, Func<object?, string>, string> mainConverter
        , Func<object?, string> addParameter)
    {
        if (ue.NodeType == ExpressionType.Convert)
        {
            return ToConvertValue(ue, mainConverter, addParameter);
        }
        throw new InvalidProgramException($"NodeType:{ue.NodeType}");
    }

    private static string ToConvertValue(UnaryExpression ue
        , Func<Expression, Func<object?, string>, string> mainConverter
        , Func<object?, string> addParameter)
    {
        var value = mainConverter(ue.Operand, addParameter);
        return FluentSelectQuery.CreateCastStatement(value, ue.Type);
    }
}
