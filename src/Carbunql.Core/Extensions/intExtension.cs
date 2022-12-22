namespace Carbunql.Core.Extensions;

internal static class intExtension
{
    public static void ForEach(this int source, Action<int> action)
    {
        for (int i = 0; i < source; i++) action(i);
    }

    public static string ToSpaceString(this int source)
    {
        var space = string.Empty;
        source.ForEach(x => space += " ");
        return space;
    }
}