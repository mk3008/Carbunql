using System.Text.RegularExpressions;

namespace Carbunql.Postgres;

internal static class StringExtension
{
    internal static string ToSnakeCase(this string input)
    {
        var cleanedInput = Regex.Replace(input, @"[^a-zA-Z0-9]", "");
        if (string.IsNullOrEmpty(cleanedInput)) return string.Empty;
        return Regex.Replace(cleanedInput, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    internal static string ToParameterName(this string input, string prefix)
    {
        var name = input.ToSnakeCase();
        if (string.IsNullOrEmpty(name)) throw new Exception("key name is empty.");

        if (!string.IsNullOrEmpty(prefix))
        {
            name = prefix.ToSnakeCase() + "_" + name;
        }

        name = DbmsConfiguration.PlaceholderIdentifier + name;
        if (name.Length > 60)
        {
            name = name.Substring(0, 60);
        }
        return name;
    }
}
