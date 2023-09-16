using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class ExistsExpressionParser
{
	public static bool IsExistsExpression(string text)
	{
		return text.IsEqualNoCase("exists");
	}

	public static ExistsExpression Parse(ITokenReader r)
	{
		r.Read("exists");
		var q = SelectQueryParser.ParseAsInner(r);
		return new ExistsExpression(q);
	}
}
