using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SortableItemParser
{
	public static SortableItem Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static SortableItem Parse(ITokenReader r)
	{
		var v = ValueParser.Parse(r);
		var isasc = true;

		if (r.Peek().AreContains(ReservedText.All()))
		{
			return new SortableItem(v);
		}

		if (r.Peek().AreEqual("asc"))
		{
			r.Read("asc");
			isasc = true;
		}
		else if (r.Peek().AreEqual("desc"))
		{
			r.Read("desc");
			isasc = false;
		}

		if (r.Peek().AreEqual("nulls first"))
		{
			r.Read("nulls first");
			return new SortableItem(v, isasc, NullSort.First);
		}

		if (r.Peek().AreEqual("nulls last"))
		{
			r.Read("nulls last");
			return new SortableItem(v, isasc, NullSort.Last);
		}

		return new SortableItem(v, isasc, NullSort.Undefined);
	}
}