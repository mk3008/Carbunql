using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Postgres;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

internal static class NewExpressionExpression
{
	internal static ValueBase ToValue(this NewExpression exp, List<string> tables)
	{
		if (exp.Type.IsAnonymousType())
		{
			var lst = new List<ValueBase>();
			for (var i = 0; i < exp.Arguments.Count; i++)
			{
				var v = exp.Arguments[i].ToValue(tables);
				v.RecommendedName = exp.Members![i].Name;
				lst.Add(v);
			}
			return new ValueCollection(lst);
		}
		else if (exp.Type == typeof(DateTime))
		{
			var args = exp.Arguments.Select(x => x.ToObject()).ToArray();
			var d = exp.Constructor!.Invoke(args);
			return ((DateTime)d).ToValue();
		}
		else if (exp.Arguments.Select(x => x is MemberExpression || x is BinaryExpression).Any())
		{
			var lst = new List<ValueBase>();
			var prms = exp.Constructor!.GetParameters().Select(p => p.Name).ToArray();

			for (var i = 0; i < exp.Arguments.Count; i++)
			{
				var v = exp.Arguments[i].ToValue(tables);
				v.RecommendedName = prms[i]!;
				lst.Add(v);
			}
			return new ValueCollection(lst);
		}
		else
		{
			var args = exp.Arguments.Select(x => x.ToObject()).ToArray();
			var d = exp.Constructor!.Invoke(args);
			return ValueParser.Parse($"'{d}'");
		}
	}

	private static object ToObject(this Expression exp)
	{
		if (exp.NodeType != ExpressionType.Constant) { throw new NotSupportedException(); }

		if (exp.Type == typeof(string)) return exp.ToString();
		if (exp.Type == typeof(int)) return int.Parse(exp.ToString());

		throw new NotSupportedException();
	}
}
