using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

public interface ITokenReader
{
	string Peek();

	string Read();

	int CurrentBracketLevel { get; }
}

public static class ITokenReaderExtension
{
	public static string Read(this ITokenReader source, string expect)
	{
		var s = source.Peek();
		if (string.IsNullOrEmpty(s)) throw new SyntaxException($"expect '{expect}', actual is empty.");
		if (!s.AreEqual(expect)) throw new SyntaxException($"expect '{expect}', actual '{s}'.");
		return source.Read();
	}

	public static string? ReadOrDefault(this ITokenReader source, string expect)
	{
		var s = source.Peek();
		if (string.IsNullOrEmpty(s)) return null;
		if (!s.AreEqual(expect)) return null;
		return source.Read();
	}

	public static string Read(this ITokenReader source, IEnumerable<string> expects)
	{
		var s = source.Peek();
		if (string.IsNullOrEmpty(s)) throw new SyntaxException($"token is empty.");
		if (!s.AreContains(expects)) throw new SyntaxException($"near '{s}'.");
		return source.Read();
	}
}