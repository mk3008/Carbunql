using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    /// <summary>
    /// Defines a set of characters considered as symbols that terminate an identifier.
    /// </summary>
    private static readonly HashSet<char> Symbols = new HashSet<char>
    {
        '+', '-', '*', '/', '%', // Arithmetic operators
        '(', ')', '[', ']', '{', '}', // Brackets and braces
        '~', '@', '#', '$', '^', '&', // Special symbols
        '!', '?', ':', ';', ',', '.', '<', '>', '=', '|', '\\', // Other symbols
        '`', '"', '\'' // Quotation marks
    };

    [MemberNotNullWhen(true)]
    private static bool TryParseCharactorValues(ReadOnlyMemory<char> memory, int position, out IEnumerable<Lex> lexes)
    {
        lexes = Enumerable.Empty<Lex>();
        var start = position;

        if (TryGetCharacterEndPosition(memory, position, out _))
        {
            lexes = ParseCharactorValues(memory, position);
            return true;
        }
        return false;
    }

    [MemberNotNullWhen(true)]
    private static IEnumerable<Lex> ParseCharactorValues(ReadOnlyMemory<char> memory, int position)
    {
        var start = position;

        if (!TryGetCharacterEndPosition(memory, position, out position))
        {
            throw new FormatException();
        }

        // check next charactor
        if (memory.IsAtEndOrWhiteSpace(position))
        {
            if (IsSpacialValueWord(memory, start, position - start))
            {
                yield return new Lex(memory, LexType.Value, start, position - start);
            }

            yield return new Lex(memory, LexType.Column, start, position - start);
            yield break;
        }

        // Identifier separator
        if (memory.Equals(position, '.', out _))
        {
            yield return new Lex(memory, LexType.Namespace, start, position - start);

            position++;//skip comma
            start = position;
            while (TryGetCharacterEndPosition(memory, position, out position))
            {
                if (memory.Equals(position, '.', out _))
                {
                    yield return new Lex(memory, LexType.Namespace, start, position - start);
                    position++;//skip comma
                    start = position;
                    continue;
                }
                else
                {
                    yield return new Lex(memory, LexType.Column, start, position - start);
                    yield break;
                }
            }
            throw new FormatException();
        }

        // left paren
        if (memory.Equals(position, '(', out _))
        {
            yield return new Lex(memory, LexType.Function, start, position - start);
            yield break;
        }

        // other
        if (IsSpacialValueWord(memory, start, position - start))
        {
            yield return new Lex(memory, LexType.Value, start, position - start);
            yield break;
        }

        yield return new Lex(memory, LexType.Column, start, position - start);
        yield break;
    }

    /// <summary>
    /// Attempts to find the end position of a valid identifier starting at a given position in the specified memory segment.
    /// </summary>
    /// <param name="memory">The memory segment containing the characters to analyze.</param>
    /// <param name="start">The initial position to start searching for the identifier.</param>
    /// <param name="position">
    /// The position marking the end of the identifier if found, or the start position if the search fails.
    /// </param>
    /// <returns>
    /// <c>true</c> if a valid identifier was found; <c>false</c> if the identifier is invalid (e.g., starts with a digit).
    /// </returns>
    private static bool TryGetCharacterEndPosition(ReadOnlyMemory<char> memory, int start, out int position)
    {
        position = start;

        // Disallow identifiers that start with a digit.
        if (char.IsDigit(memory.Span[position]))
        {
            return false;
        }

        // Loop until the end of the identifier or a symbol/whitespace is encountered.
        while (!memory.IsAtEnd(position))
        {
            var current = memory.Span[position];
            if (char.IsWhiteSpace(current) || Symbols.Contains(current))
            {
                break;
            }
            position++;
        }

        // Return false if no valid identifier was found.
        return (position != start);
    }
}