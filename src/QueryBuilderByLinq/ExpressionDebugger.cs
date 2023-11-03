using Carbunql.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace QueryBuilderByLinq;

public static class ExpressionDebugger
{

	public static void WriteImmediate(Expression exp)
	{
		var s = Analyze(exp);
		System.Diagnostics.Debug.WriteLine(s);
	}

	public static string Analyze(Expression exp)
	{
		if (exp is MethodCallExpression m)
		{
			return Analyze(m);
		}
		else if (exp is ConstantExpression c)
		{
			return Analyze(c);
		}
		else if (exp is UnaryExpression u)
		{
			return Analyze(u);
		}
		else if (exp is LambdaExpression l)
		{
			return Analyze(l);
		}
		else if (exp is MemberExpression mem)
		{
			return Analyze(mem);
		}
		else if (exp is ParameterExpression p)
		{
			return Analyze(p);
		}
		return $"not support : {exp.NodeType}";
	}

	public static string Analyze(MethodCallExpression exp)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* MethodCallExpression");
		sb.AppendLine($"NodeType\r\n    {exp.NodeType}");
		sb.AppendLine($"MethodName\r\n    {exp.Method.Name}");
		sb.AppendLine($"Type\r\n    {exp.Type.Name}");

		sb.AppendLine($"Object");
		if (exp.Object != null)
		{
			sb.Append(Analyze(exp.Object).InsertIndent());
		}
		else
		{
			sb.AppendLine($"    [NULL]");
		}

		var cnt = exp.Arguments.Count;
		sb.AppendLine($"Arguments.Count\r\n    {cnt}");

		foreach (var arg in exp.Arguments)
		{
			sb.AppendLine($"- index : {exp.Arguments.IndexOf(arg)}");
			sb.Append(Analyze(arg).InsertIndent());
		}

		return sb.ToString().RemoveLastReturn();
	}

	public static string Analyze(ConstantExpression exp)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* ConstantExpression");
		sb.AppendLine($"NodeType\r\n    {exp.NodeType}");
		sb.AppendLine($"Type\r\n    {exp.Type.Name}");

		if (exp.Value != null)
		{
			sb.AppendLine($"Value\r\n    {exp.Value.ToString()}");
		}
		else
		{
			sb.AppendLine($"Value\r\n    [NULL]");
		}
		return sb.ToString().RemoveLastReturn();
	}

	public static string Analyze(UnaryExpression exp)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* UnaryExpression");
		sb.AppendLine($"NodeType\r\n    {exp.NodeType}");
		sb.AppendLine($"Type\r\n    {exp.Type.Name}");

		if (exp.Method != null)
		{
			sb.AppendLine($"Method");
			sb.AppendLine(Analyze(exp.Method).InsertIndent());
		}
		else
		{
			sb.AppendLine($"Method\r\n    [NULL]");
		}

		sb.AppendLine($"Operand");
		sb.AppendLine(Analyze(exp.Operand).InsertIndent());



		return sb.ToString().RemoveLastReturn();
	}

	public static string Analyze(LambdaExpression exp)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* LambdaExpression");
		sb.AppendLine($"NodeType\r\n    {exp.NodeType}");
		sb.AppendLine($"Type\r\n    {exp.Type.Name}");
		sb.AppendLine($"Name\r\n    \"{exp.Name}\"");

		sb.AppendLine($"ReturnType\r\n    {exp.ReturnType}");
		sb.AppendLine($"Body");
		sb.AppendLine(Analyze(exp.Body).InsertIndent());

		sb.AppendLine($"Parameters count\r\n    {exp.Parameters.Count}");
		foreach (var arg in exp.Parameters)
		{
			sb.AppendLine($"- index : {exp.Parameters.IndexOf(arg)}");
			sb.AppendLine(Analyze(arg).InsertIndent());
		}

		return sb.ToString().RemoveLastReturn();
	}

	public static string Analyze(MemberExpression exp)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* MemberExpression");
		sb.AppendLine($"NodeType\r\n    {exp.NodeType}");
		sb.AppendLine($"Type\r\n    {exp.Type.Name}");
		sb.AppendLine($"Member");
		sb.AppendLine(Analyze(exp.Member).InsertIndent());

		if (exp.Expression != null)
		{
			sb.AppendLine($"Expression");
			sb.AppendLine(Analyze(exp.Expression).InsertIndent());
		}
		else
		{
			sb.AppendLine($"Expression\r\n    [NULL]");
		}

		var s = sb.ToString();
		return sb.ToString().RemoveLastReturn();
	}

	public static string Analyze(ParameterExpression exp)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* MemberExpression");
		sb.AppendLine($"NodeType\r\n    {exp.NodeType}");
		sb.AppendLine($"Type\r\n    {exp.Type.Name}");
		sb.AppendLine($"Name\r\n    {exp.Name}");

		return sb.ToString().RemoveLastReturn();
	}


	public static string Analyze(MemberInfo info)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* MemberInfo");
		sb.AppendLine($"Name\r\n    {info.Name}");
		sb.AppendLine($"MemberType\r\n    {info.MemberType}");

		var s = sb.ToString().RemoveLastReturn();
		return sb.ToString().RemoveLastReturn();
	}

	public static string Analyze(MethodInfo info)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* MethodInfo");
		sb.AppendLine($"Name\r\n    {info.Name}");
		sb.AppendLine($"ReturnType\r\n    {info.ReturnType.Name}");

		var prms = info.GetParameters().ToList();

		sb.AppendLine($"Parameters count\r\n    {prms.Count()}");
		foreach (var p in prms)
		{
			sb.AppendLine($"- index : {prms.IndexOf(p)}");
			sb.AppendLine(Analyze(p).InsertIndent());
		}

		return sb.ToString().RemoveLastReturn();
	}

	public static string Analyze(ParameterInfo info)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"* ParameterInfo");
		sb.AppendLine($"Name\r\n    {info.Name}");
		sb.AppendLine($"ParameterType\r\n    {info.ParameterType.Name}");

		return sb.ToString().RemoveLastReturn();
	}

	internal static string RemoveLastReturn(this string s)
	{
		return Regex.Replace(s, @"\r\n$", "");
	}
}
