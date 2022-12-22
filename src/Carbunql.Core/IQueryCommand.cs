namespace Carbunql.Core;

public interface IQueryCommand
{
    IEnumerable<Token> GetTokens(Token? parent);
}