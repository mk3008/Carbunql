using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class InExpressionParser
{
	public static InExpression Parse(ValueBase value, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(value, r);
	}

	public static InExpression Parse(ValueBase value, ITokenReader r)
	{
		r.Read("(");
		using var ir = new BracketInnerTokenReader(r);
		var first = ir.Peek();
		if (first == null) throw new NotSupportedException();

		if (first.AreEqual("select"))
		{
			//sub query
			var iq = new InlineQuery(SelectQueryParser.Parse(ir));
			return new InExpression(value, iq);
		}
		else
		{
			//value collection
			var bv = new BracketValue(ValueCollectionParser.Parse(ir));
			return new InExpression(value, bv);
		}
	}
}