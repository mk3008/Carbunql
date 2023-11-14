using Carbunql.Building;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Analysis;

internal static class ConditionalExpressionExtension
{
	internal static CaseExpression ToValue(this ConditionalExpression exp, List<string> tables)
	{
		var test = exp.Test.ToValue(tables);
		var truevale = exp.IfTrue.ToValue(tables);

		var cw = new CaseExpression();
		cw.WhenExpressions.Add(new WhenExpression(test, truevale));

		var v = exp.IfFalse.ToValue(tables);
		if (v is CaseExpression c)
		{
			foreach (var item in c.WhenExpressions)
			{
				cw.WhenExpressions.Add(item);
			}
		}
		else
		{
			cw.Else(v);
		}
		return cw;
	}
}
