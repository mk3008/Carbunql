using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class RelationParser
{
	public static Relation Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static Relation Parse(ITokenReader r)
	{
		if (r.PeekRawToken().AreContains((x) =>
		{
			if (x.AreEqual(ReservedText.Cross)) return true;
			if (x.AreEqual(ReservedText.Comma)) return true;
			return false;
		}))
		{
			var join = r.ReadToken();
			var table = SelectableTableParser.Parse(r);
			return new Relation(table, join);
		}
		else if (r.PeekRawToken().AreContains((x) =>
		{
			if (x.AreEqual(ReservedText.Inner)) return true;
			if (x.AreEqual(ReservedText.Left)) return true;
			if (x.AreEqual(ReservedText.Right)) return true;
			return false;
		}))
		{
			var join = r.ReadToken();
			var table = SelectableTableParser.Parse(r);
			r.ReadToken("on");
			var val = ValueParser.Parse(r);
			return new Relation(table, join, val);
		}

		throw new SyntaxException("");
	}
}