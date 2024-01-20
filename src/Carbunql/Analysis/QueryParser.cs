using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class QueryParser
{
	public static IReadQuery Parse(string text)
	{
		using var r = new TokenReader(text);
		var q = Parse(r);

		if (!r.Peek().IsEndToken())
		{
			throw new NotSupportedException($"Parsing terminated despite the presence of unparsed tokens.(token:'{r.Peek()}')");
		}

		return q;
	}

	public static IReadQuery Parse(ITokenReader r)
	{
		if (r.Peek().IsEqualNoCase("with")) return CTEQueryParser.Parse(r);
		if (r.Peek().IsEqualNoCase("select")) return SelectQueryParser.Parse(r);
		if (r.Peek().IsEqualNoCase("values")) return ValuesQueryParser.Parse(r);

		throw new NotSupportedException();
	}
}