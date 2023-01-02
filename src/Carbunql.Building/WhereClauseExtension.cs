using Carbunql.Clauses;

namespace Carbunql.Building;

public static class WhereClauseExtension
{
    public static void Where(this SelectQuery source, Func<ValueBase> builder)
    {
        if (source.WhereClause == null)
        {
            source.WhereClause = new WhereClause(builder());
        }
        else
        {
            var v = source.WhereClause.Condition.GetLast();
            v.And(builder());
        }
    }
}
