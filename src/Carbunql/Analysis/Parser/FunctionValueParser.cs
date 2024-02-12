using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class FunctionValueParser
{
	public static FunctionValue Parse(string text, string functionName)
	{
		var r = new SqlTokenReader(text);
		return Parse(r, functionName);
	}

	public static FunctionValue Parse(ITokenReader r, string functionName)
	{
		var arg = ValueCollectionParser.ParseAsInner(r);

		Filter? filter = null;
		OverClause? over = null;

		if (r.Peek().IsEqualNoCase("filter"))
		{
			r.Read("filter");
			filter = FilterParser.Parse(r);
		}

		if (r.Peek().IsEqualNoCase("over"))
		{
			r.Read("over");
			over = OverClauseParser.Parse(r);
		}

		var fnc = new FunctionValue(functionName, arg);
		fnc.Filter = filter;
		fnc.Over = over;

		return fnc;
	}
}