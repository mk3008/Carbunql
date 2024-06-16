using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class UnaryExpressionExtension
{
    internal static string ToValue(this UnaryExpression ue
        , Func<string, object?, string> addParameter)
    {
        if (ue.NodeType == ExpressionType.Convert)
        {
            return ToConvertValue(ue, addParameter);
        }
        throw new InvalidProgramException($"NodeType:{ue.NodeType}");
    }

    private static string ToConvertValue(UnaryExpression ue
        , Func<string, object?, string> addParameter)
    {
        var value = ue.Operand.ToValue(addParameter);
        return FluentSelectQuery.CreateCastStatement(value, ue.Type);
    }
}
