using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class FunctionTableParser
{
	public static FunctionTable Parse(string text, string functionName)
	{
		using var r = new TokenReader(text);
		return Parse(r, functionName);
	}

	public static FunctionTable Parse(ITokenReader r, string functionName)
	{
		var arg = ValueCollectionParser.ParseAsInner(r);

		return new FunctionTable(functionName, arg);
	}
}