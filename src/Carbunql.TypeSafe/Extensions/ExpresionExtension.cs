using System.Linq.Expressions;

namespace Carbunql.TypeSafe.Extensions;

public static class ExpresionExtension
{
    public static string ToValue(this Expression exp, Func<string, object?, string> addParameter)
    {
        if (exp is MemberExpression mem)
        {
            return mem.ToValue(addParameter);
        }
        else if (exp is ConstantExpression ce)
        {
            return ce.ToValue(addParameter);
        }
        else if (exp is NewExpression ne)
        {
            return ne.ToValue(addParameter);
        }
        else if (exp is BinaryExpression be)
        {
            return be.ToValue(addParameter);
        }
        else if (exp is UnaryExpression ue)
        {
            return ue.ToValue(addParameter);
        }
        else if (exp is MethodCallExpression mce)
        {
            return mce.ToValue(addParameter);
        }
        else if (exp is ConditionalExpression cnd)
        {
            return cnd.ToValue(addParameter);
        }
        else if (exp is ParameterExpression prm)
        {
            return prm.ToValue(addParameter);
        }

        throw new InvalidProgramException(exp.ToString());
    }
}
