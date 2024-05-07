using Carbunql.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Token reader for reading tokens within brackets.
/// </summary>
public class BracketInnerTokenReader : ITokenReader, IDisposable
{
    /// <summary>
    /// Gets or sets the start symbol indicating the beginning of the bracket.
    /// </summary>
    private string StartSymbol { get; init; } = "(";

    /// <summary>
    /// Gets or sets the end symbol indicating the end of the bracket.
    /// </summary>
    private string EndSymbol { get; init; } = ")";

    /// <summary>
    /// Initializes a new instance of the <see cref="BracketInnerTokenReader"/> class with custom start and end symbols.
    /// </summary>
    /// <param name="r">The token reader to use.</param>
    /// <param name="startSymbol">The start symbol indicating the beginning of the bracket.</param>
    /// <param name="endSymbol">The end symbol indicating the end of the bracket.</param>
    public BracketInnerTokenReader(ITokenReader r, string startSymbol, string endSymbol)
    {
        StartSymbol = startSymbol;
        EndSymbol = endSymbol;

        r.Read(StartSymbol);

        Reader = r;
        RootBracketLevel = r.CurrentBracketLevel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BracketInnerTokenReader"/> class with default start and end symbols.
    /// </summary>
    /// <param name="r">The token reader to use.</param>
    public BracketInnerTokenReader(ITokenReader r)
    {
        r.Read(StartSymbol);

        Reader = r;
        RootBracketLevel = r.CurrentBracketLevel;
    }

    /// <summary>
    /// Gets the token reader.
    /// </summary>
    private ITokenReader Reader { get; set; }

    /// <summary>
    /// Gets the root bracket level.
    /// </summary>
    private int RootBracketLevel { get; set; }

    /// <inheritdoc/>
    public int CurrentBracketLevel => Reader.CurrentBracketLevel;

    /// <summary>
    /// Gets or sets a value indicating whether the reading process is terminated.
    /// </summary>
    private bool IsTerminated { get; set; } = false;

    /// <inheritdoc/>
    public string Peek()
    {
        if (IsTerminated) return string.Empty;

        return Reader.Peek();
    }

    /// <inheritdoc/>
    public string Read()
    {
        if (IsTerminated) return string.Empty;

        var token = Reader.Read();
        if (token == EndSymbol && RootBracketLevel > Reader.CurrentBracketLevel)
        {
            IsTerminated = true;
        }
        return token;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Reader.Read(EndSymbol);
    }

    /// <inheritdoc/>
    public void RollBack()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
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
