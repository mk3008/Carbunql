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

        var prmManager = new ParameterManager(GetParameters(), AddParameter);

        if (body.Members != null)
        {
            var cnt = body.Members.Count();
            for (var i = 0; i < cnt; i++)
            {
                var alias = body.Members[i].Name;
                var value = ToValue(body.Arguments[i], prmManager.AddParaemter);
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

        var prmManager = new ParameterManager(GetParameters(), AddParameter);

        var value = ToValue(expression.Body, prmManager.AddParaemter);

        if (expression.Body is BinaryExpression be && be.NodeType == ExpressionType.OrElse)
        {
            this.Where($"({value})");
        }
        else
        {
            this.Where(value);
        }
        return this;
    }

    private string ToValue(Expression exp, Func<string, object?, string> addParameter)
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

    internal static string CreateCastStatement(string value, Type type)
    {
        var dbtype = DbmsConfiguration.ToDbType(type);
        return $"cast({value} as {dbtype})";
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
