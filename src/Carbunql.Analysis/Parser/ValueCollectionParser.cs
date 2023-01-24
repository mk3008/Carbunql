using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class ValueCollectionParser
{
	public static ValueCollection Parse(string text)
	{
		using var r = new TokenReader(text);
		return new ValueCollection(ReadValues(r).ToList());
	}

	public static ValueCollection Parse(ITokenReader r)
	{
		return new ValueCollection(ReadValues(r).ToList());
	}

	public static ValueCollection ParseAsInner(ITokenReader r)
	{
		r.TryReadToken("(");
		var ir = new InnerTokenReader(r);
		var v = new ValueCollection(ReadValues(ir).ToList());
		return v;
	}

	internal static IEnumerable<ValueBase> ReadValues(ITokenReader r)
	{
		do
		{
			if (r.PeekRawToken().AreEqual(",")) r.ReadToken();
			yield return ValueParser.Parse(r);
		}
		while (r.PeekRawToken().AreEqual(","));
	}
}