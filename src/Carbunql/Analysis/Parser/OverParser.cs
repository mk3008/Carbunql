using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class OverParser
{
	public static Over Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static Over ParseAsInner(ITokenReader r)
	{
		r.Read("(");
		using var ir = new BracketInnerTokenReader(r);
		var v = Parse(ir);
		return v;
	}

	public static Over Parse(ITokenReader r)
	{
		r.ReadOrDefault("(");
		var token = r.Read();

		var winfn = new Over();
		do
		{
			if (token.IsEqualNoCase("partition by"))
			{
				winfn.PartitionBy = PartitionClauseParser.Parse(r);
			}
			else if (token.IsEqualNoCase("order by"))
			{
				winfn.OrderBy = OrderClauseParser.Parse(r);
			}
			token = r.Read();
		} while (!string.IsNullOrEmpty(token) && !token.IsEqualNoCase(")"));

		return winfn;
	}
}