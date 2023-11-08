using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class SelectableItemParser
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

		if (operand.Body is NewExpression)
		{
			//select custom column pattern.
			var body = (NewExpression)operand.Body;
			var val = body.ToValue(aliases);
			results = Parse(val).ToList();
			return results;
		}
		if (operand.Body is ParameterExpression)
		{
			//select all pattern.
			var body = (ParameterExpression)operand.Body;
			var val = body.ToValue(aliases);
			results = Parse(val).ToList();
			return results;
		}
		if (operand.Body is MemberExpression)
		{
			//join and select all pattern.
			var body = (MemberExpression)operand.Body;
			if (body == null) throw new NotSupportedException();
			var val = body.ToValue(aliases);

			if (val is ColumnValue c && c.Column == "*")
			{
				return DecodeWildCard(exp, c).ToList();
			}

			results = Parse(val).ToList();
			return results;

		}

		throw new NotSupportedException();
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

	private static IEnumerable<SelectableItem> DecodeWildCard(Expression exp, ColumnValue v)
	{
		var tableinfo = TableInfoParser.Parse(exp);

		if (tableinfo!.Alias == v.TableAlias)
		{
			foreach (var item in tableinfo!.Table!.GetColumnNames())
			{
				yield return new SelectableItem(new ColumnValue(v.TableAlias, item), item);
			}
			yield break;
		}

		var joinInfo = JoinTableInfoParser.Parse(exp).Where(x => x.TableInfo!.Alias == v.TableAlias).FirstOrDefault();
		if (joinInfo != null)
		{
			foreach (var item in joinInfo.TableInfo.Table!.GetColumnNames())
			{
				yield return new SelectableItem(new ColumnValue(v.TableAlias, item), item);
			}
			yield break;
		}

		throw new NotSupportedException();
	}
}
