﻿using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

internal static class InvocationExpressionExtension
{
	internal static ParameterValue ToParameterValue(this InvocationExpression exp)
	{
		var value = exp.Execute();

		var inner = exp.Expression;
		var key = ((MemberExpression)inner).Member.Name.ToParameterName("invoke");

		var prm = new ParameterValue(key, value);
		return prm;
	}
}
