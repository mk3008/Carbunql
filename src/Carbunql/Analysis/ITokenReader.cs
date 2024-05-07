using Carbunql.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Represents a token reader interface for reading tokens.
/// </summary>
public interface ITokenReader
{
    /// <summary>
    /// Peeks the next token without consuming it.
    /// </summary>
    /// <returns>The next token.</returns>
    string Peek();

    /// <summary>
    /// Reads the next token and consumes it.
    /// </summary>
    /// <returns>The read token.</returns>
    string Read();

    /// <summary>
    /// Rolls back the last read token.
    /// </summary>
    void RollBack();

    /// <summary>
    /// Tries to read the next token and match it with the expected token.
    /// </summary>
    /// <param name="expect">The expected token.</param>
    /// <param name="token">The actual token read.</param>
    /// <returns>True if the actual token matches the expected token, otherwise false.</returns>
    bool TryRead(string expect, [MaybeNullWhen(false)] out string token);

    /// <summary>
    /// Gets the current bracket level.
    /// </summary>
    int CurrentBracketLevel { get; }
}

/// <summary>
/// Provides extension methods for the ITokenReader interface.
/// </summary>
public static class ITokenReaderExtension
{
    /// <summary>
    /// Reads the next token and throws an exception if it does not match the expected token.
    /// </summary>
    /// <param name="source">The ITokenReader instance.</param>
    /// <param name="expect">The expected token.</param>
    /// <returns>The read token.</returns>
    public static string Read(this ITokenReader source, string expect)
    {
        var s = source.Read();
        if (string.IsNullOrEmpty(s)) throw new SyntaxException($"Expected '{expect}', but the actual token is empty.");
        if (!s.IsEqualNoCase(expect)) throw new SyntaxException($"Expected '{expect}', but the actual token is '{s}'.");
        return s;
    }

    /// <summary>
    /// Reads the next token if it matches the expected token, otherwise returns null.
    /// </summary>
    /// <param name="source">The ITokenReader instance.</param>
    /// <param name="expect">The expected token.</param>
    /// <returns>The read token if it matches the expected token, otherwise null.</returns>
    public static string? ReadOrDefault(this ITokenReader source, string expect)
    {
        var s = source.Peek();
        if (!s.IsEqualNoCase(expect)) return null;
        return source.Read();
    }
}
