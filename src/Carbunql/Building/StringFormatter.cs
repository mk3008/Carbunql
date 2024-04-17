using Cysharp.Text;

namespace Carbunql.Building;

public static class StringFormatter
{
    private static char[] UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public static string ToUpperSnakeCase(this string value) => ToSnakeCase(value).ToUpper();

    public static string ToLowerSnakeCase(this string value) => ToSnakeCase(value).ToLower();

    public static string ToSnakeCase(this string value)
    {
        using var sb = ZString.CreateStringBuilder();
        var isPrevUpper = false;

        foreach (var item in value)
        {
            var isupper = UpperChars.Contains(item);
            if (sb.Length == 0)
            {
                sb.Append(item);
            }
            else if (isupper && !isPrevUpper)
            {
                sb.Append("_");
                sb.Append(item);
            }
            else
            {
                sb.Append(item);
            }
            isPrevUpper = isupper;
        }
        return sb.ToString();
    }
}