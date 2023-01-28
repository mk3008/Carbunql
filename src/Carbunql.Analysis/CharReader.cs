using Cysharp.Text;

namespace Carbunql.Analysis;

public class CharReader : IDisposable
{
	private bool disposedValue;

	public CharReader(string text)
	{
		Text = text;
		Reader = new StringReader(Text);
	}

	public string Text { get; init; }

	private StringReader Reader { get; init; }

	protected char? PeekOrDefaultChar()
	{
		var i = Reader.Peek();
		if (i < 0) return null;
		return (char)i;
	}

	public char ReadChar()
	{
		var i = Reader.Read();
		if (i < 0) throw new EndOfStreamException();
		return (char)i;
	}

	public char? TryReadChar(char expect)
	{
		var c = PeekOrDefaultChar();
		if (c == expect) return ReadChar();
		return null;
	}

	public IEnumerable<char> ReadChars() => ReadChars((_) => true);

	public IEnumerable<char> ReadChars(Func<char, bool> whileFn)
	{
		var c = PeekOrDefaultChar();
		while (c != null && whileFn(c.Value))
		{
			yield return ReadChar();
			c = PeekOrDefaultChar();
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