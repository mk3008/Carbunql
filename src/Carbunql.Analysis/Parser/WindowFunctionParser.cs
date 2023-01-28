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
		return v;
	}

	public static WindowFunction Parse(ITokenReader r)
	{
		r.TryReadToken("(");
		var token = r.ReadToken();

		var winfn = new WindowFunction();
		do
		{
			if (token.AreEqual("partition by"))
			{
				winfn.PartitionBy = PartitionClauseParser.Parse(r);
			}
			else if (token.AreEqual("order by"))
			{
				winfn.OrderBy = OrderClauseParser.Parse(r);
			}
			token = r.ReadToken();
		} while (!string.IsNullOrEmpty(token) && !token.AreEqual(")"));

		return winfn;
	}
}