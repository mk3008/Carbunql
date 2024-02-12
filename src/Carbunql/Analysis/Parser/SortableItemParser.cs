using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SortableItemParser
{
	public static SortableItem Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static SortableItem Parse(ITokenReader r)
	{
		var v = ValueParser.Parse(r);
		var isasc = true;

		if (r.Peek().IsEqualNoCase(ReservedText.All()))
		{
			return new SortableItem(v);
		}

		if (r.Peek().IsEqualNoCase("asc"))
		{
			r.Read("asc");
			isasc = true;
		}
		else if (r.Peek().IsEqualNoCase("desc"))
		{
			r.Read("desc");
			isasc = false;
		}

		if (r.Peek().IsEqualNoCase("nulls first"))
		{
			r.Read("nulls first");
			return new SortableItem(v, isasc, NullSort.First);
		}

		if (r.Peek().IsEqualNoCase("nulls last"))
		{
			r.Read("nulls last");
			return new SortableItem(v, isasc, NullSort.Last);
		}

		return new SortableItem(v, isasc, NullSort.Undefined);
	}
}