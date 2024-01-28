using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql.Analysis.Parser;

public static class LimitClauseParser
{
	public static LimitClause Parse(string text)
	{
		using var r = new SqlTokenReader(text);
		return Parse(r);
	}

	public static LimitClause Parse(ITokenReader r)
	{
		var condition = ParseItems(r).ToList();
		if (r.ReadOrDefault("offset") != null)
		{
			var offset = ValueParser.Parse(r);
			return new LimitClause(condition) { Offset = offset };
		}
		return new LimitClause(condition);
	}

	private static IEnumerable<ValueBase> ParseItems(ITokenReader r)
	{
		do
		{
			if (r.Peek().IsEqualNoCase(",")) r.Read();
			yield return ValueParser.Parse(r);
		}
		while (r.Peek().IsEqualNoCase(","));
	}
}