using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class SelectClauseParser
{
	public static SelectClause Parse(string text)
	{
		var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static SelectClause Parse(ITokenReader r)
	{
		r.ReadOrDefault("select");

		DistinctClause? distinct = null;
		TopClause? top = null;
		if (r.Peek().IsEqualNoCase("distinct"))
		{
			distinct = DistinctClauseParser.Parse(r);
		}
		if (r.Peek().IsEqualNoCase("top"))
		{
			top = TopParser.Parse(r);
		}
		return new SelectClause(ParseItems(r).ToList())
		{
			Distinct = distinct,
			Top = top
		};
	}

	private static IEnumerable<SelectableItem> ParseItems(ITokenReader r)
	{
		do
		{
			if (r.Peek().IsEqualNoCase(",")) r.Read();
			yield return SelectableItemParser.Parse(r);
		}
		while (r.Peek().IsEqualNoCase(","));
	}
}