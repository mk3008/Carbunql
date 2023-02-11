using Carbunql.Values;

namespace Carbunql.Building;

public static class StringExtension
{
	public static ValueCollection ToValueCollection(this IList<string> source)
	{
		var vals = new ValueCollection();
		foreach (var item in source)
		{
			vals.Add(new LiteralValue(item));
		}
		return vals;
	}
}