namespace Carbunql;

public static class IQueryCommandExtension
{
    public static IEnumerable<Token> GetTokens(this IQueryCommand source)
    {
        return source.GetTokens(null);
    }
}