using Carbunql.Clauses;

namespace Carbunql.Analysis.Parser;

public static class WithoutTImeZoneClauseParser
{
	public static WithoutTimeZoneClause Parse(ValueBase value, ITokenReader r)
	{
		r.Read("without time zone");
		return new WithoutTimeZoneClause(value);
	}
}