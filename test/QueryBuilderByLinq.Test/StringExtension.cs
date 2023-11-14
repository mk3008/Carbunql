namespace QueryBuilderByLinq.Test;

internal static class StringExtension
{
	public static string RemoveControlChar(this string source)
	{
		return source.Replace("\n", "").Replace("\r", "").Replace("\t", "").ToLower();
	}
}
