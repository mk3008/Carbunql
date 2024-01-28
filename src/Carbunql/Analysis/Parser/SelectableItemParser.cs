using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectableItemParser
{
	public static SelectableItem Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static SelectableItem Parse(ITokenReader r)
	{
		var v = ValueParser.Parse(r);
		r.ReadOrDefault("as");

		if (r.Peek().IsEqualNoCase(ReservedText.All()))
		{
			return new SelectableItem(v, v.GetDefaultName());
		}

		return new SelectableItem(v, r.Read());
	}
}