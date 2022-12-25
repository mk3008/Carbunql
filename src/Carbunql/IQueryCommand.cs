namespace Carbunql;

public interface IQueryCommand
{
	IEnumerable<Token> GetTokens(Token? parent);
}