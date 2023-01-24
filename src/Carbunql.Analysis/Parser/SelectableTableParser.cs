using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectableTableParser
{
	private static string[] SelectTableBreakTokens = new[] { "on" };

	public static SelectableTable Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	private static bool ReservedTokenFilter(string text)
	{
		if (ReservedText.As == text) return false;
		return true;
	}

	public static SelectableTable Parse(ITokenReader r)
	{
		var v = TableParser.Parse(r);
		var t = r.PeekRawToken();
		if (t == null || t.AreContains(ReservedText.All(ReservedTokenFilter)))
		{
			return new SelectableTable(v, v.GetDefaultName());
		}

		r.TryReadToken("as");
		var alias = r.ReadToken();

		if (!r.PeekRawToken().AreEqual("("))
		{
			return new SelectableTable(v, alias);
		}

		var colAliases = ValueCollectionParser.ParseAsInner(r);

		return new SelectableTable(v, alias, colAliases);
	}
}