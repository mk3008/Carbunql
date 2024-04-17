using Cysharp.Text;

namespace Carbunql.Extensions;

internal static class intExtension
{
    public static void ForEach(this int source, Action<int> action)
    {
        for (int i = 0; i < source; i++) action(i);
    }

    public static string ToSpaceString(this int source)
    {
        using var sb = ZString.CreateStringBuilder();
        source.ForEach(s => sb.Append(" "));
        return sb.ToString();
    }
}