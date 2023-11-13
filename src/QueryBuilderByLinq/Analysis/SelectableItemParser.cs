using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class SelectableItemParser
{
	public static List<SelectableItem> Parse(Expression exp)
	{
		var tableinfo = SelectableTableParser.Parse(exp);
		var joinInfos = JoinTableInfoParser.Parse(exp);

		var aliases = new List<SelectableTable>();
		if (tableinfo != null) aliases.Add(tableinfo);
		foreach (var item in joinInfos)
		{
			aliases.Add(item.Table);
		}

		return Parse(exp, aliases);
	}

	public static List<SelectableItem> Parse(Expression exp, List<SelectableTable> aliases)
	{
		var results = new List<SelectableItem>();
		if (exp is not MethodCallExpression) return results;

		var me = (MethodCallExpression)exp;
		if (me.Arguments.Count == 2)
		{
			var operand = me.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
			if (operand == null) return results;

			if (operand.Body is NewExpression)
			{
				//select custom column pattern.
				var body = (NewExpression)operand.Body;
				var val = body.ToValue(aliases.Select(x => x.Alias).ToList());
				results = Parse(val).ToList();
				return results;
			}
			if (operand.Body is ParameterExpression)
			{
				//select all pattern.
				var body = (ParameterExpression)operand.Body;
				var val = body.ToValue(aliases.Select(x => x.Alias).ToList());
				results = Parse(val).ToList();
				return results;
			}
			if (operand.Body is MemberExpression)
			{
				//join and select all pattern.
				var body = (MemberExpression)operand.Body;
				if (body == null) throw new NotSupportedException();
				var val = body.ToValue(aliases.Select(x => x.Alias).ToList());

				if (val is ColumnValue c && c.Column == "*")
				{
					return DecodeWildCard(aliases, c).ToList();
				}

				results = Parse(val).ToList();
				return results;
			}
			if (operand.ReturnType == typeof(bool) && operand.Parameters.Count == 1)
			{
				//select and where pattern.
				var prm = operand.GetParameter<ParameterExpression>(0);
				if (prm == null) throw new NotSupportedException();

				var val = prm.ToValue(aliases.Select(x => x.Alias).ToList());
				results = Parse(val).ToList();
				return results;
			}

			throw new NotSupportedException();
		}

		if (me.Arguments.Count == 3)
		{
			var operand = me.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
			if (operand == null) return results;

			if (operand.Body is NewExpression)
			{
				//select custom column pattern.
				var body = (NewExpression)operand.Body;
				var val = body.ToValue(aliases.Select(x => x.Alias).ToList());
				results = Parse(val).ToList();
				return results;
			}
			if (operand.Body is ParameterExpression)
			{
				//select all pattern.
				var body = (ParameterExpression)operand.Body;
				var val = body.ToValue(aliases.Select(x => x.Alias).ToList());
				results = Parse(val).ToList();
				return results;
			}
			if (operand.Body is MemberExpression)
			{
				//join and select all pattern.
				var body = (MemberExpression)operand.Body;
				if (body == null) throw new NotSupportedException();
				var val = body.ToValue(aliases.Select(x => x.Alias).ToList());

				if (val is ColumnValue c && c.Column == "*")
				{
					return DecodeWildCard(aliases, c).ToList();
				}

				results = Parse(val).ToList();
				return results;
			}

			throw new NotSupportedException();
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

	private static IEnumerable<SelectableItem> DecodeWildCard(List<SelectableTable> aliases, ColumnValue v)
	{
		var t = aliases.Where(x => x.Alias == v.TableAlias).FirstOrDefault();
		if (t == null) throw new NotSupportedException();

		foreach (var item in t.GetColumnNames())
		{
			yield return new SelectableItem(new ColumnValue(v.TableAlias, item), item);
		}
	}
}
