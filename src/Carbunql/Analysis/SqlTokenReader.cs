namespace Carbunql.Analysis;

/// <summary>
/// Class for reading tokens. Reading concludes when an SQL terminator is encountered.
/// </summary>
public class SqlTokenReader : TokenReader, ITokenReader
{
	public SqlTokenReader(string text) : base(text)
	{
	}

	private bool IsTeminated { get; set; } = false;

	private string Cache { get; set; } = string.Empty;

	/// <summary>
	/// Method to peek at a token.
	/// </summary>
	/// <returns>The token.</returns>
	public string Peek()
	{
		if (IsTeminated) return string.Empty;

		if (string.IsNullOrEmpty(Cache))
		{
			Cache = base.Read();
		}
		return Cache;
	}

	private void Commit()
	{
		Cache = string.Empty;
	}

	/// <summary>
	/// Method to read a token.
	/// </summary>
	/// <returns>The token.</returns>
	public override string Read()
	{
		if (IsTeminated) return string.Empty;

		var token = Peek();
		Commit();

		if (token == ";")
		{
			IsTeminated = true;
		}

		return token;
	}
}