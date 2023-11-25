namespace Carbunql.Building.Test;

public static class StringExtension
{
	public static string ToValidateText(this string text)
	{
		return text.Replace("\r", "")
			.Replace("\n", "")
			.Replace(" ", "")
			.Replace("\t", "")
			.ToLower();
	}
}
