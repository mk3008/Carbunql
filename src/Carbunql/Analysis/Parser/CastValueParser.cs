using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class CastValueParser
{
	public static bool IsCastValue(string text)
	{
		return text ==  "::";
	}

	public static CastValue Parse(ValueBase value, string symbol, string argument)
	{
		using var r = new SqlTokenReader(argument);
		return Parse(value, symbol, r);
	}

	public static CastValue Parse(ValueBase value, string symbol, ITokenReader r)
	{
		var type = ValueParser.Parse(r);
		return new CastValue(value, symbol, type);
	}
}