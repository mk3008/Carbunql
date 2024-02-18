namespace Carbunql.Postgres.Test;

public static class Extension
{
	public static string ToValidateText(this string source)
	{
		return source.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
	}


}
