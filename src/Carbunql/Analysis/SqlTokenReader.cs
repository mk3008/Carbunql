namespace Carbunql.Analysis;

public class SqlTokenReader : TokenReader, ITokenReader
{
	public SqlTokenReader(string text) : base(text)
	{
	}

	private bool IsTeminated { get; set; } = false;

	private string TokenCache { get; set; } = string.Empty;

	public string Peek()
	{
		if (IsTeminated) return string.Empty;

		if (!string.IsNullOrEmpty(TokenCache)) return TokenCache;

		TokenCache = base.Read();
		return TokenCache;
	}

	public override string Read()
	{
		if (IsTeminated) return string.Empty;

		if (!string.IsNullOrEmpty(TokenCache))
		{
			var s = TokenCache;
			TokenCache = string.Empty;
			return s;
		}

		var token = base.Read();

		if (token == ";")
		{
			IsTeminated = true;
		}

		return token;
	}
}