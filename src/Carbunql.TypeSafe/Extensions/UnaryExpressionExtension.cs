using Carbunql.TypeSafe.Building;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

internal static class UnaryExpressionExtension
{
    internal static string ToValue(this UnaryExpression ue, BuilderEngine engine)
    {
        if (ue.NodeType == ExpressionType.Convert)
        {
            return ToConvertValue(ue, engine);
        }
        throw new InvalidProgramException($"NodeType:{ue.NodeType}");
    }

    private static string ToConvertValue(UnaryExpression ue, BuilderEngine engine)
    {
        var value = ue.Operand.ToValue(engine);
        return engine.SqlDialect.GetCastStatement(value, ue.Type);
    }
}
