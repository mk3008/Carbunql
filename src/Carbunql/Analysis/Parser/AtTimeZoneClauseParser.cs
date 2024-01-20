using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class AtTimeZoneClauseParser
{
	public static AtTimeZoneClause Parse(ValueBase value, ITokenReader r)
	{
		r.Read("at time zone");
		var timezone = ValueParser.Parse(r);
		return new AtTimeZoneClause(value, timezone);
	}
}