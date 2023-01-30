using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectableItemParser
{
	public static SelectableItem Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static SelectableItem Parse(ITokenReader r)
	{
		var v = ValueParser.Parse(r);
		r.ReadOrDefault("as");

		if (r.Peek().AreContains(ReservedText.All()))
		{
			return new SelectableItem(v, v.GetDefaultName());
		}

		return new SelectableItem(v, r.Read());
	}
}