using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class BracketValueParser
{
	public static bool IsBracketValue(string text)
	{
		return text == "(";
	}

	public static ValueBase Parse(ITokenReader r)
	{
		using var ir = new BracketInnerTokenReader(r);

		var pt = ir.Peek();

		if (pt.IsEqualNoCase("select"))
		{
			var q = SelectQueryParser.Parse(ir);
			return new InlineQuery(q);
		}
		else
		{
			var v = ValueParser.Parse(ir);
			return new BracketValue(v);
		}
	}
}
