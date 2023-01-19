using Carbunql.Extensions;

namespace Carbunql.Analysis;

public interface ITokenReader
{
	string ReadToken(bool skipComment = true);

	string? PeekRawToken(bool skipComment = true);

	string ReadToken(string expectRawToken);

	(string first, string inner) ReadUntilCloseBracket();

	string? TryReadToken(string expectRawToken);

	string ReadUntilToken(string breaktoken);

	string ReadToken(string[] expectRawTokens);

	string ReadUntilToken(Func<string, bool> fn);

	string ReadUntilCaseExpressionEnd();
}