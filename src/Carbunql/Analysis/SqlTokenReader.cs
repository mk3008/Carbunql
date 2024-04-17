using Carbunql.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Class for reading tokens. Reading concludes when an SQL terminator is encountered.
/// </summary>
public class SqlTokenReader : TokenReader, ITokenReader
{
    public SqlTokenReader(string text) : base(text)
    {
    }

    public bool TryReadNextQuery([MaybeNullWhen(false)] out string peekToken)
    {
        peekToken = null;
        if (Peek() == ";")
        {
            Read();
        }
        IsTeminated = false;
        peekToken = Peek();
        if (string.IsNullOrEmpty(peekToken))
        {
            return false;
        }
        return true;
    }

    private bool IsTeminated { get; set; } = false;

    private string Cache { get; set; } = string.Empty;

    private string RollBackCache { get; set; } = string.Empty;

    private string ReadedCache { get; set; } = string.Empty;

    /// <summary>
    /// Method to peek at a token.
    /// </summary>
    /// <returns>The token.</returns>
    public string Peek()
    {
        if (IsTeminated) return string.Empty;

        if (string.IsNullOrEmpty(Cache))
        {
            if (!string.IsNullOrEmpty(ReadedCache))
            {
                Cache = ReadedCache;
                ReadedCache = string.Empty;
            }
            else
            {
                Cache = base.Read();
            }
        }
        return Cache;
    }

    private void Commit()
    {
        if (!string.IsNullOrEmpty(Cache))
        {
            RollBackCache = Cache;
            Cache = string.Empty;
        }
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

    public void RollBack()
    {
        if (string.IsNullOrEmpty(RollBackCache)) throw new Exception("fail");
        if (string.IsNullOrEmpty(Cache)) throw new Exception("fail");
        ReadedCache = Cache;
        Cache = RollBackCache;
        RollBackCache = string.Empty;
    }

    public bool TryRead(string expect, [MaybeNullWhen(false)] out string token)
    {
        token = null;
        var t = Peek();
        if (t.IsEqualNoCase(expect))
        {
            token = Read();
            return true;
        }
        return false;
    }
}