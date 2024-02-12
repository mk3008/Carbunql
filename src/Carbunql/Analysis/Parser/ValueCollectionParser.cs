using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class ValueCollectionParser
{
	public static ValueCollection Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return new ValueCollection(ReadValues(r).ToList());
	}

	public static ValueCollection Parse(ITokenReader r)
	{
		return new ValueCollection(ReadValues(r).ToList());
	}

	public static ValueCollection ParseAsInner(ITokenReader r)
	{
		using var ir = new BracketInnerTokenReader(r);
		if (ir.Peek() == ")") return new ValueCollection();

		var v = new ValueCollection(ReadValues(ir).ToList());

		return v;
	}

	internal static IEnumerable<ValueBase> ReadValues(ITokenReader r)
	{
		do
		{
			if (r.Peek().IsEqualNoCase(",")) r.Read();
			var v = ValueParser.Parse(r);

			if (r.ReadOrDefault("from") != null)
			{
				yield return FromArgumentParser.Parse(v, r);
			}
			else if (r.ReadOrDefault("as") != null)
			{
				yield return AsArgumentParser.Parse(v, r);
			}
			else
			{
				yield return v;
			}
		}
		while (r.Peek().IsEqualNoCase(","));
	}
}