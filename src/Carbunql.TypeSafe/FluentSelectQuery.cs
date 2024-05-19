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

        throw new InvalidProgramException(exp.ToString());
    }

    internal static string CreateCastStatement(string value, Type type)
    {
        var dbtype = DbmsConfiguration.ToDbType(type);
        return $"cast({value} as {dbtype})";
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
