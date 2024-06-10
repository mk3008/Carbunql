using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

public static class ExpresionExtension
{
    public static string ToValue(this Expression exp, Func<string, object?, string> addParameter)
    {
        if (exp is MemberExpression mem)
        {
            return mem.ToValue(ToValue, addParameter);
        }
        else if (exp is ConstantExpression ce)
        {
            return ce.ToValue(ToValue, addParameter);
        }
        else if (exp is NewExpression ne)
        {
            return ne.ToValue(ToValue, addParameter);
        }
        else if (exp is BinaryExpression be)
        {
            return be.ToValue(ToValue, addParameter);
        }
        else if (exp is UnaryExpression ue)
        {
            return ue.ToValue(ToValue, addParameter);
        }
        else if (exp is MethodCallExpression mce)
        {
            return mce.ToValue(ToValue, addParameter);
        }
        else if (exp is ConditionalExpression cnd)
        {
            return cnd.ToValue(ToValue, addParameter);
        }
        else if (exp is ParameterExpression prm)
        {
            return prm.ToValue(ToValue, addParameter);
        }

        throw new InvalidProgramException(exp.ToString());
    }
}
