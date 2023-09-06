using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

public static class CaseExtension
{
	public static WhenExpression When(this CaseExpression source, string condition)
	{
		var w = new WhenExpression(ValueParser.Parse(condition), new LiteralValue("null"));
		source.WhenExpressions.Add(w);
		return w;
	}

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

	public static void Then(this WhenExpression source, string value)
	{
		source.SetValue(ValueParser.Parse(value));
	}

	public static void Then(this WhenExpression source, ValueBase value)
	{
		source.SetValue(value);
	}

	public static void Else(this CaseExpression source, string value)
	{
		var w = new WhenExpression(ValueParser.Parse(value));
		source.WhenExpressions.Add(w);
	}

	public static void Else(this CaseExpression source, ValueBase value)
	{
		var w = new WhenExpression(value);
		source.WhenExpressions.Add(w);
	}
}
