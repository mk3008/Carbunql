using Carbunql.TypeSafe.Building;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

public static class ExpresionExtension
{
    internal static string ToValue(this Expression exp, BuilderEngine engine)
    {
        if (exp is MemberExpression mem)
        {
            return mem.ToValue(engine);
        }
        else if (exp is ConstantExpression ce)
        {
            return ce.ToValue(engine);
        }
        else if (exp is NewExpression ne)
        {
            return ne.ToValue(engine);
        }
        else if (exp is BinaryExpression be)
        {
            return be.ToValue(engine);
        }
        else if (exp is UnaryExpression ue)
        {
            return ue.ToValue(engine);
        }
        else if (exp is MethodCallExpression mce)
        {
            return mce.ToValue(engine);
        }
        else if (exp is ConditionalExpression cnd)
        {
            return cnd.ToValue(engine);
        }
        else if (exp is ParameterExpression prm)
        {
            return prm.ToValue(engine);
        }

        throw new InvalidProgramException(exp.ToString());
    }
}
