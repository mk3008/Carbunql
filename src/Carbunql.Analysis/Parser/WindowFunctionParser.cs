using Carbunql.Extensions;
using Carbunql.Values;
using static System.Net.Mime.MediaTypeNames;

namespace Carbunql.Analysis.Parser;

public static class WindowFunctionParser
{
	public static WindowFunction Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static WindowFunction ParseAsInner(ITokenReader r)
	{
		r.ReadToken("(");
		var ir = new InnerTokenReader(r);
		var v = Parse(ir);
		r.ReadToken(")");
		return v;
	}

	public static WindowFunction Parse(ITokenReader r)
	{
		var winfn = new WindowFunction();

		do
		{
			var token = r.ReadToken();
			if (token.AreEqual("partition by"))
			{
				winfn.PartitionBy = PartitionClauseParser.Parse(r);
			}
			else if (token.AreEqual("order by"))
			{
				winfn.OrderBy = OrderClauseParser.Parse(r);
			}
		} while (r.PeekRawToken() != null);

		return winfn;
	}
}