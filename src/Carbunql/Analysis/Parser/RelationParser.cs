using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class RelationParser
{
	public static Relation Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static Relation Parse(ITokenReader r)
	{
		if (r.Peek().IsEqualNoCase((x) =>
		{
			if (x.IsEqualNoCase(ReservedText.Cross)) return true;
			if (x.IsEqualNoCase(ReservedText.Comma)) return true;
			return false;
		}))
		{
			var join = r.Read();
			var table = SelectableTableParser.Parse(r);
			return new Relation(table, join);
		}
		else if (r.Peek().IsEqualNoCase((x) =>
		{
			if (x.IsEqualNoCase(ReservedText.Join)) return true;
			if (x.IsEqualNoCase(ReservedText.Inner)) return true;
			if (x.IsEqualNoCase(ReservedText.Left)) return true;
			if (x.IsEqualNoCase(ReservedText.Right)) return true;
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