using Carbunql.Extensions;

namespace Carbunql.Analysis;

public interface ITokenReader
{
	string? PeekRawToken(bool skipComment = true);

	string? ReadRawToken(bool skipSpace = true);

	string ReadToken(bool skipComment = true);

	string ReadUntilToken(Func<string, bool> fn);

	(string first, string inner) ReadUntilCloseBracket();
}

public static class ITokenReaderExtension
{
	public static string ReadToken(this ITokenReader source, string expectRawToken)
	{
		var s = source.PeekRawToken();
		if (string.IsNullOrEmpty(s)) throw new SyntaxException($"expect '{expectRawToken}', actual is empty.");
		if (!s.AreEqual(expectRawToken)) throw new SyntaxException($"expect '{expectRawToken}', actual '{s}'.");
		return source.ReadToken();
	}

	public static string ReadToken(this ITokenReader source, string[] expectRawTokens)
	{
		var s = source.PeekRawToken();
		if (string.IsNullOrEmpty(s)) throw new SyntaxException($"token is empty.");
		if (!s.AreContains(expectRawTokens)) throw new SyntaxException($"near '{s}'.");
		return source.ReadToken();
	}

	public static string? TryReadToken(this ITokenReader source, string expectRawToken)
	{
		var s = source.PeekRawToken();
		if (!s.AreEqual(expectRawToken)) return null;
		return source.ReadToken();
	}

	public static string ReadUntilToken(this ITokenReader source, string breaktoken)
	{
		return source.ReadUntilToken(x => x.AreEqual(breaktoken));
	}


	public static IEnumerable<string> ReadRawTokens(this ITokenReader source, bool skipSpace = true)
	{
		var token = source.ReadRawToken(skipSpace: skipSpace);
		while (!string.IsNullOrEmpty(token))
		{
			yield return token;
			token = source.ReadRawToken(skipSpace: skipSpace);
		}
	}
}