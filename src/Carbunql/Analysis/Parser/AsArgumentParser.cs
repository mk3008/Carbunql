using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class AsArgumentParser
{
	public static AsArgument Parse(ValueBase value, string argument)
	{
		var r = new SqlTokenReader(argument);
		return Parse(value, r);
	}

	public static AsArgument Parse(ValueBase value, ITokenReader r)
	{
		var type = ValueParser.Parse(r);
		return new AsArgument(value, type);
	}
}
