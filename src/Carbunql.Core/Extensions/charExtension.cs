namespace Carbunql.Core.Extensions;

public static class charExtension
{
    public static bool IsInteger(this char source)
    {
        return "0123456789".Contains(source);
    }
}