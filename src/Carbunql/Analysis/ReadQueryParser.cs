using Carbunql.Extensions;

namespace Carbunql.Analysis;

public static class ReadQueryParser
{
	public static ReadQuery Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static ReadQuery Parse(ITokenReader r)
	{
		if (r.Peek().IsEqualNoCase("select")) return SelectQueryParser.Parse(r);
		if (r.Peek().IsEqualNoCase("values")) return ValuesQueryParser.Parse(r);

		throw new NotSupportedException(r.Peek());
	}
}