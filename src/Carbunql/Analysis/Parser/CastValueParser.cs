using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Analysis.Parser;

public static class CastValueParser
{
	public static CastValue Parse(ValueBase value, string symbol, string argument)
	{
		using var r = new TokenReader(argument);
		return Parse(value, symbol, r);
	}

	public static CastValue Parse(ValueBase value, string symbol, ITokenReader r)
	{
		var type = ValueParser.Parse(r);
		return new CastValue(value, symbol, type);
	}
}