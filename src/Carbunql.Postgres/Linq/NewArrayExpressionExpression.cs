using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

internal static class NewArrayExpressionExpression
{
	internal static ValueCollection ToValue(this NewArrayExpression exp, List<string> tables)
	{
		var vc = new ValueCollection();
		foreach (var item in exp.Expressions)
		{
			vc.Add(item.ToValue(tables));
		}
		return vc;
	}
}
