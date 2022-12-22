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

    public IEnumerable<char> SpaceChars { get; set; } = " \r\n\t".ToArray();

    public string Text { get; init; }

    private StringReader Reader { get; init; }

    public char? PeekOrDefault()
    {
        var i = Reader.Peek();
        if (i < 0) return null;
        return (char)i;
    }

    public bool PeekAreEqual(char expect) => PeekOrDefault() == expect;

    public IEnumerable<char> ReadUntil(Func<char, bool> untilFn) => ReadWhile(x => !untilFn(x));

    public IEnumerable<char> ReadWhile(Func<char, bool> whileFn)
    {
        var c = PeekOrDefault();
        while (c != null && whileFn(c.Value))
        {
            yield return ReadChar();
            c = PeekOrDefault();
        }
    }

    public IEnumerable<char> ReadChars() => ReadWhile((_) => true);

    public char ReadChar()
    {
        var i = Reader.Read();
        if (i < 0) throw new EndOfStreamException();
        return (char)i;
    }

    public string ReadUntilSingleQuote()
    {
        using var sb = ZString.CreateStringBuilder();
        foreach (var item in ReadChars())
        {
            sb.Append(item);
            if (item == '\'')
            {
                if (PeekAreEqual('\''))
                {
                    sb.Append(ReadChar());
                    continue;
                }
                return sb.ToString();
            }
        }
        throw new SyntaxException("single quote is not closed.");
    }

    public void SkipSpace() => ReadWhile(x => SpaceChars.Contains(x)).ToList();

    public string ReadUntilLineEnd()
    {
        using var sb = ZString.CreateStringBuilder();
        foreach (var item in ReadChars())
        {
            if (item != '\n' && item != '\r')
            {
                sb.Append(item);
                continue;
            }

            if (item == '\r')
            {
                var c = PeekOrDefault();
                if (c != null && c.Value == '\n') ReadChar();
            }
            break;
        }
        return sb.ToString();
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