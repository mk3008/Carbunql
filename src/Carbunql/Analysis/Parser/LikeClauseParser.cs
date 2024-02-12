using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Analysis.Parser;

public static class LikeClauseParser
{
	public static LikeClause Parse(ValueBase value, string argument)
	{
		var r = new SqlTokenReader(argument);
		return Parse(value, r, false);
	}

	public static LikeClause Parse(ValueBase value, ITokenReader r, bool isNegative)
	{
		r.Read("like");

		var argument = ValueParser.ParseCore(r);
		return new LikeClause(value, argument, isNegative);
	}
}