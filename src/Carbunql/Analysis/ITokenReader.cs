using Carbunql.Extensions;

namespace Carbunql.Analysis;

public interface ITokenReader
{
	string Peek();

	string Read();

	void RollBack();

	int CurrentBracketLevel { get; }
}

public static class ITokenReaderExtension
{
	public static string Read(this ITokenReader source, string expect)
	{
		var s = source.Read();
		if (string.IsNullOrEmpty(s)) throw new SyntaxException($"expect '{expect}', actual is empty");
		if (!s.IsEqualNoCase(expect)) throw new SyntaxException($"expect '{expect}', actual '{s}'");
		return s;
	}

	// TODO TryRead
	public static string? ReadOrDefault(this ITokenReader source, string expect)
	{
		var s = source.Peek();
		if (!s.IsEqualNoCase(expect)) return null;
		return source.Read();
	}
}