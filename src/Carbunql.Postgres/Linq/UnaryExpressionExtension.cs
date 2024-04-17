using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

internal static class UnaryExpressionExtension
{
    internal static ValueBase ToValue(this UnaryExpression exp, List<string> tables)
    {
        var v = exp.ToValueCore(tables);
        if (exp.NodeType == ExpressionType.Convert) return v;
        if (exp.NodeType == ExpressionType.Quote) return v;
        if (exp.NodeType == ExpressionType.Not)
        {
            if (v is ExistsExpression) return new NegativeValue(v);
            if (v is InClause ic)
            {
                ic.IsNegative = true;
                return ic;
            }
            if (v is LikeClause lc)
            {
                lc.IsNegative = true;
                return lc;
            }
            return new NegativeValue(v.ToBracket());
        }
        throw new NotSupportedException();
    }

    private static ValueBase ToValueCore(this UnaryExpression exp, List<string> tables)
    {
        if (exp.Operand is MemberExpression mem)
        {
            return mem.ToValue(tables);
        }

        if (exp.Operand is ConstantExpression cons)
        {
            return cons.ToValue();
        }

        if (exp.Operand is MethodCallExpression ce)
        {
            return ce.ToValue(tables);
        }

        if (exp.Operand is ConditionalExpression cond)
        {
            return cond.ToValue(tables);
        }

        if (exp.Operand is BinaryExpression binary)
        {
            return binary.ToValueExpression(tables);
        }

        if (exp.NodeType == ExpressionType.Convert)
        {
            return exp.ToCastValue(tables);
        }

        if (exp.NodeType == ExpressionType.Quote)
        {
            var lexp = exp.Execute() as LambdaExpression;
            if (lexp == null) throw new InvalidProgramException();
            return lexp.Body.ToValue(tables);
        }

        throw new NotSupportedException(exp.Operand.ToString());
    }

    private static ValueBase ToCastValue(this UnaryExpression exp, List<string> tables)
    {
        var v = exp.Operand.ToValue(tables);

        if (exp.Operand.Type.ToTryDbType(out var dbType))
        {
            return new CastValue(v, "::", dbType);
        }
        return v;
    }
}
