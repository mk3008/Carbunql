using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class SelectColumnInfoParser
{
	public static List<SelectableItem> Parse(Expression exp)
	{
		var tableinfo = TableInfoParser.Parse(exp);
		var joinInfos = JoinTableInfoParser.Parse(exp);

		var aliases = new List<string>();
		if (tableinfo != null) aliases.Add(tableinfo.Alias);
		foreach (var item in joinInfos)
		{
			aliases.Add(item.TableInfo.Alias);
		}

		return Parse(exp, aliases);
	}

	public static List<SelectableItem> Parse(Expression exp, List<string> aliases)
	{
		var results = new List<SelectableItem>();
		if (exp is not MethodCallExpression) return results;

		var me = (MethodCallExpression)exp;
		if (me.Arguments.Count != 2) return results;

		var operand = me.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (operand == null) return results;

		var body = operand.GetBody<NewExpression>();
		if (body == null) return results;

		var val = body.ToValue(aliases);

		results = Parse(val).ToList();
		return results;
	}

	internal static IEnumerable<SelectableItem> Parse(ValueBase value)
	{
		if (value is ValueCollection vc)
		{
			foreach (var v in Parse(vc))
			{
				yield return v;
			}
		}
		else
		{
			var alias = !string.IsNullOrEmpty(value.RecommendedName) ? value.RecommendedName : value.GetDefaultName();
			yield return new SelectableItem(value, alias);
		}
	}

	internal static IEnumerable<SelectableItem> Parse(ValueCollection collection)
	{
		foreach (var item in collection)
		{
			foreach (var v in Parse(item))
			{
				yield return v;
			}
		}
	}

}
