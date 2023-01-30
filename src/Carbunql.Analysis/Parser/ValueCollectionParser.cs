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
		r.ReadOrDefault("(");

		// no argument. '()'
		if (r.Peek().AreEqual(")"))
		{
			r.Read(")");
			return new ValueCollection();
		}

		var ir = new BracketInnerTokenReader(r);
		var v = new ValueCollection(ReadValues(ir).ToList());
		r.ReadOrDefault(")");
		return v;
	}

	internal static IEnumerable<ValueBase> ReadValues(ITokenReader r)
	{
		do
		{
			if (r.Peek().AreEqual(",")) r.Read();
			yield return ValueParser.Parse(r);
		}
		while (r.Peek().AreEqual(","));
	}
}