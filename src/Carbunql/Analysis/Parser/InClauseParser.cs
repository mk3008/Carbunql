using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class InClauseParser
{
	public static InClause Parse(ValueBase value, string argument)
	{
		var r = new SqlTokenReader(argument);
		return Parse(value, r, false);
	}

	public static InClause Parse(ValueBase value, ITokenReader r, bool isNegative)
	{
		r.Read("in");

		using var ir = new BracketInnerTokenReader(r);

		var first = ir.Peek() ?? throw new NotSupportedException();
		if (first.IsEqualNoCase("select"))
		{
			//sub query
			var iq = new InlineQuery(SelectQueryParser.Parse(ir));
			return new InClause(value, iq, isNegative);
		}
		else
		{
			//value collection
			var bv = new BracketValue(ValueCollectionParser.Parse(ir));
			return new InClause(value, bv, isNegative);
		}
	}
}