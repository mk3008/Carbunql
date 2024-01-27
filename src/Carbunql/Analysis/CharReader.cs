using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Class that processes text character by character.
/// Reading can only be done in the forward direction, and it cannot be reversed.
/// </summary>
public class CharReader : IDisposable
{
	private bool disposedValue;

	/// <summary>
	/// Constructor for the CharReader class. Initializes a StringReader using the specified text.
	/// </summary>
	/// <param name="text">The text to be processed</param>
	public CharReader(string text)
	{
		Reader = new StringReader(text);
	}

	private StringReader Reader { get; init; }

	private char? PeekOrDefault()
	{
		var i = Reader.Peek();
		if (i < 0) return null;
		return (char)i;
	}

	private char Read()
	{
		var i = Reader.Read();
		if (i < 0) throw new EndOfStreamException();
		return (char)i;
	}

	/// <summary>
	/// Reads one character. Returns false if reading is not possible.
	/// </summary>
	/// <param name="c">The character that was read</param>
	/// <returns>True if reading was successful, otherwise false.</returns>
	public bool TryRead([MaybeNullWhen(false)] out char c)
	{
		c = default;

		var i = Reader.Peek();
		if (i < 0) return false;
		c = Read();
		return true;
	}

	/// <summary>
	/// Reads one character. Reading is canceled and false is returned if the read character is different from the expected value.
	/// </summary>
	/// <param name="expect">The expected value</param>
	/// <param name="c">The character that was read</param>
	/// <returns>True if the read character matches the expected value, otherwise false</returns>
	public bool TryRead(char expect, [MaybeNullWhen(false)] out char c)
	{
		c = default;

		var i = Reader.Peek();
		if (i < 0) return false;
		if ((char)i == expect)
		{
			c = Read();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Returns one character at a time until the end.
	/// </summary>
	/// <returns>The read character.</returns>
	public IEnumerable<char> Reads() => Reads((_) => true);

	/// <summary>
	/// Returns one character at a time while the specified condition is met.
	/// </summary>
	/// <param name="whileFn">The condition to continue reading</param>
	/// <returns>The read character.</returns>
	public IEnumerable<char> Reads(Func<char, bool> whileFn)
	{
		var c = PeekOrDefault();
		while (c != null && whileFn(c.Value))
		{
			yield return Read();
			c = PeekOrDefault();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: マネージド状態を破棄します (マネージド オブジェクト)
				Reader.Dispose();
			}

			// TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
			// TODO: 大きなフィールドを null に設定します
			disposedValue = true;
		}
	}

	// // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
	// ~CharReader()
	// {
	//     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}