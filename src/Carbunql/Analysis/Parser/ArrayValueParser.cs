using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class ArrayValueParser
{
	public static ArrayValue Parse(string argument)
	{
		var r = new SqlTokenReader(argument);
		return Parse(r);
	}

	public static ArrayValue Parse(ITokenReader r)
	{
		r.Read("array");
		using var ir = new BracketInnerTokenReader(r, "[", "]");

		var collection = ValueCollectionParser.Parse(ir);
		return new ArrayValue(collection);
	}
}