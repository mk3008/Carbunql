using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

internal static class ValuesClauseParser
{
	public static ValuesQuery Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static ValuesQuery Parse(ITokenReader r)
	{
		var fn = () =>
		{
			if (!r.Peek().IsEqualNoCase(",")) return false;
			r.Read(",");
			return true;
		};

		r.ReadOrDefault("values");

		var lst = new List<ValueCollection>();
		do
		{
			lst.Add(ValueCollectionParser.ParseAsInner(r));
		} while (fn());

		return new ValuesQuery(lst);
	}
}