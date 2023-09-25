namespace Carbunql.Building.Test;

public static class Extension
{
	public static string ToValidateText(this string source)
	{
		return source.Replace("\r", "").Replace("\n", "").Replace(" ", "");
	}
}
