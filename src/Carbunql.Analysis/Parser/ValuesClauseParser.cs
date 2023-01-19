using Carbunql.Clauses;
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
			if (!r.PeekRawToken().AreEqual(",")) return false;
			r.ReadToken(",");
			r.ReadToken("(");
			return true;
		};

		r.TryReadToken("values");
		r.ReadToken("(");

		var lst = new List<ValueCollection>();
		do
		{
			var (_, inner) = r.ReadUntilCloseBracket();
			lst.Add(ValueCollectionParser.Parse(inner));
		} while (fn());

		return new ValuesQuery(lst);
	}
}