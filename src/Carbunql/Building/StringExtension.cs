using Carbunql.Analysis.Parser;
using Carbunql.Values;

namespace Carbunql.Building;

public static class StringExtension
{
	public static ValueCollection ToValueCollection(this IList<string> source)
	{
		var vals = new ValueCollection();
		foreach (var item in source)
		{
			vals.Add(ValueParser.Parse(item));
		}
		return vals;
	}

	public static ValueCollection ToValueCollection(this IList<string> source, string alias)
	{
		var vals = new ValueCollection();
		foreach (var item in source)
		{
			vals.Add(new ColumnValue(alias, item));
		}
		return vals;
	}
}