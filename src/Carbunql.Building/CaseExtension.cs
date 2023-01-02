using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class CaseExtension
{
    public static WhenExpression When(this CaseExpression source, ValueBase condition)
    {
        var w = new WhenExpression(condition, new LiteralValue("null"));
        source.WhenExpressions.Add(w);
        return w;
    }

    public static WhenExpression When(this CaseExpression source, Func<ValueBase> builder)
    {
        var w = new WhenExpression(builder(), new LiteralValue("null"));
        source.WhenExpressions.Add(w);
        return w;
    }

    public static void Then(this WhenExpression source, ValueBase value)
    {
        source.SetValue(value);
    }

    public static void Else(this CaseExpression source, ValueBase value)
    {
        var w = new WhenExpression(value);
        source.WhenExpressions.Add(w);
    }
}
