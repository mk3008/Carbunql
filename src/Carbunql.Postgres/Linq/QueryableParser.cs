using Carbunql.Postgres;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

internal static class Queryable
{
    public static bool TryParse(ConstantExpression @const, out IQueryable query)
    {
        query = null!;

        if (@const.Value is IQueryable q && q.Provider is TableQuery tq)
        {
            if (tq.InnerQuery != null)
            {
                query = tq.InnerQuery;
                return true;
            }
        }
        return false;
    }

    public static bool TryParse(MethodCallExpression method, out IQueryable query)
    {
        query = null!;

        if (!method.Arguments.Any()) return false;

        if (!(method.Method.Name == nameof(Sql.InnerJoinTable) || method.Method.Name == nameof(Sql.LeftJoinTable) || method.Method.Name == nameof(Sql.CrossJoinTable) || method.Method.Name == nameof(Sql.CommonTable)))
        {
            return false;
        }

        if (method.Arguments[0] is MemberExpression mem && mem.Expression is ConstantExpression ce)
        {
            var fieldname = mem.Member.Name;
            var val = ce.Value;
            if (val == null) return false;
            var tp = val.GetType();
            if (!tp.GetFields().Any()) return false;
            var field = tp.GetFields().Where(x => x.Name == fieldname).FirstOrDefault();
            if (field == null) return false;
            if (field.GetValue(ce.Value) is IQueryable q)
            {
                query = q;
                return true;
            }
        }

        return false;
    }
}
