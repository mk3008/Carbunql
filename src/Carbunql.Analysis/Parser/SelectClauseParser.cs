﻿using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectClauseParser
{
	public static SelectClause Parse(string text)
	{
		using var r = new TokenReader(text);
		return Parse(r);
	}

	public static SelectClause Parse(ITokenReader r)
	{
		r.TryReadToken("select");

		var distinct = r.TryReadToken("distinct") != null ? true : false;
		if (r.TryReadToken("top") == null)
		{
			return new SelectClause(ParseItems(r).ToList()) { HasDistinctKeyword = distinct };
		}
		var top = ValueParser.Parse(r);
		return new SelectClause(ParseItems(r).ToList()) { HasDistinctKeyword = distinct, Top = top };
	}

	private static IEnumerable<SelectableItem> ParseItems(ITokenReader r)
	{
		do
		{
			if (r.PeekRawToken().AreEqual(",")) r.ReadToken();
			yield return SelectableItemParser.Parse(r);
		}
		while (r.PeekRawToken().AreEqual(","));
	}
}