using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbunql.Postgres.Analysis;

internal static class MemberExpressionExtension
{
	internal static ValueBase ToValue(this MemberExpression exp, List<string> tables)
	{
		if (exp.ToString() == "DateTime.Now")
		{
			return ValueParser.Parse("current_timestamp");
		}

		if (exp.Expression is null && exp.Member is PropertyInfo prop)
		{
			if (prop.GetGetMethod()!.IsStatic)
			{
				var d = prop!.GetValue(null);
				if (exp.Type == typeof(DateTime) && d != null)
				{
					return ((DateTime)d).ToValue();
				}
			}
		}

		if (exp.Expression is null)
		{
			throw new NotSupportedException($"Expression is null.");
		}

		if (exp.Expression is ParameterExpression prm)
		{
			if (prm.Name!.StartsWith("<>h__TransparentIdentifier"))
			{
				return new ColumnValue(exp.Member.Name, "*");
			}
			var table = prm.Name!;
			var column = exp.Member.Name;
			return new ColumnValue(table, column);
		}

		if (exp.Expression is MemberExpression mem)
		{
			var table = tables.Where(x => x == mem.Member.Name).FirstOrDefault();
			if (mem.Member.Name.StartsWith("<>h__TransparentIdentifier"))
			{
				return new ColumnValue(exp.Member.Name, "*");
			}

			if (!string.IsNullOrEmpty(table))
			{
				var column = exp.Member.Name;
				return new ColumnValue(table, column);
			}

			if (mem.Expression != null)
			{
				return mem.ToValue(tables);
			}

			return exp.ToValue();
		}

		if (exp.Expression is ConstantExpression)
		{
			return exp.ToValue();
		}

		throw new NotSupportedException($"propExpression.Expression type:{exp.Expression.GetType().Name}");
	}

	private static ValueBase ToValue(this MemberExpression exp)
	{
		object? value = null;
		if (exp.Member is FieldInfo field && exp.Expression is ConstantExpression ce)
		{
			value = field.GetValue(ce.Value);
		}
		else
		{
			value = exp.Execute();
		}

		if (value is IEnumerable<ValueBase> values)
		{
			var vc = new ValueCollection();
			foreach (var item in values)
			{
				vc.Add(item);
			}
			return vc;
		}

		return exp.ToParameterValue(value);
	}

	private static ValueBase ToParameterValue(this MemberExpression exp, object? value)
	{
		var key = string.Empty;

		if (exp.Expression is MemberExpression m)
		{
			key = m.Member.Name + '_' + exp.Member.Name;
		}
		else
		{
			key = exp.Member.Name;
		}
		key = key.ToParameterName("member");

		var prm = new ParameterValue(key, value);
		return prm;
	}
}
