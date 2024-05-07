using Carbunql.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Class for reading SQL tokens. Reading concludes when an SQL terminator is encountered.
/// </summary>
public class SqlTokenReader : TokenReader, ITokenReader
{
    /// <summary>
    /// Constructor for the SqlTokenReader class.
    /// </summary>
    /// <param name="text">The SQL text to be read.</param>
    public SqlTokenReader(string text) : base(text)
    {
    }

    /// <summary>
    /// Tries to read the next SQL query if the current token is an SQL terminator.
    /// </summary>
    /// <param name="peekToken">The next token after peeking.</param>
    /// <returns>True if the next token is available, false otherwise.</returns>
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

    /// <summary>
    /// Indicates whether the reading process is terminated.
    /// </summary>
    private bool IsTeminated { get; set; } = false;

    /// <summary>
    /// Cache for storing the current token.
    /// </summary>
    private string Cache { get; set; } = string.Empty;

    /// <summary>
    /// Cache for storing the token for rollback.
    /// </summary>
    private string RollBackCache { get; set; } = string.Empty;

    /// <summary>
    /// Cache for storing the previously read token.
    /// </summary>
    private string ReadedCache { get; set; } = string.Empty;

    /// <summary>
    /// Peeks at the next token.
    /// </summary>
    /// <returns>The next token.</returns>
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

    /// <summary>
    /// Commits the current token and marks it as read.
    /// </summary>
    private void Commit()
    {
        if (!string.IsNullOrEmpty(Cache))
        {
            RollBackCache = Cache;
            Cache = string.Empty;
        }
    }

    /// <summary>
    /// Reads the next token.
    /// </summary>
    /// <returns>The next token.</returns>
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

    /// <summary>
    /// Rolls back the read process to the last token.
    /// </summary>
    public void RollBack()
    {
        if (string.IsNullOrEmpty(RollBackCache)) throw new Exception("fail");
        if (string.IsNullOrEmpty(Cache)) throw new Exception("fail");
        ReadedCache = Cache;
        Cache = RollBackCache;
        RollBackCache = string.Empty;
    }

    /// <summary>
    /// Attempts to read the specified token.
    /// </summary>
    /// <param name="expect">The expected token.</param>
    /// <param name="token">The read token.</param>
    /// <returns>True if the expected token is read, false otherwise.</returns>
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
