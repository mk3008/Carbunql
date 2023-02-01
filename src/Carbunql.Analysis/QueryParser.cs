using Carbunql.Analysis.Parser;
using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class QueryParser
{
	public static IReadQuery Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static IReadQuery Parse(ITokenReader r)
	{
		if (r.Peek().AreEqual("with")) return CTEQueryParser.Parse(r);
		if (r.Peek().AreEqual("select")) return SelectQueryParser.Parse(r);
		if (r.Peek().AreEqual("values")) return ValuesQueryParser.Parse(r);

		throw new NotSupportedException();
	}
}