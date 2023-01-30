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
		if (r.Peek().AreContains((x) =>
		{
			if (x.AreEqual(ReservedText.Cross)) return true;
			if (x.AreEqual(ReservedText.Comma)) return true;
			return false;
		}))
		{
			var join = r.Read();
			var table = SelectableTableParser.Parse(r);
			return new Relation(table, join);
		}
		else if (r.Peek().AreContains((x) =>
		{
			if (x.AreEqual(ReservedText.Inner)) return true;
			if (x.AreEqual(ReservedText.Left)) return true;
			if (x.AreEqual(ReservedText.Right)) return true;
			return false;
		}))
		{
			var join = r.Read();
			var table = SelectableTableParser.Parse(r);
			r.Read("on");
			var val = ValueParser.Parse(r);
			return new Relation(table, join, val);
		}

		throw new SyntaxException("");
	}
}