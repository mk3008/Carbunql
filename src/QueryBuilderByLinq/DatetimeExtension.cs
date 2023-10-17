using Carbunql.Clauses;
using Carbunql.Values;

namespace QueryBuilderByLinq;

internal static class DatetimeExtension
{
	internal static ValueBase ToValue(this DateTime d)
	{
		return new FunctionValue("cast", new CastValue(new LiteralValue($"'{d}'"), "as", new LiteralValue("timestamp")));
	}
}
