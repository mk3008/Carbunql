using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static HashSet<string> SpecialValueWords = new HashSet<string>
    {
        "true",
        "false",
        "null",

        "current_timestamp",
        "current_date",
        "current_time",
        "timestamp",
        "now",

        "yesterday",
        "today",
        "tomorrow",

        "-infinity",
        "infinity",
        "allballs",
        "epoch",
    };

    [MemberNotNullWhen(true)]
    public static bool TryParseCharactorValue(ReadOnlyMemory<char> memory, int start, out Lex lex, out int endPosition)
    {
        lex = default;
        var position = start;
        endPosition = start;

        // Escaped Column pattern
        if (memory.IsAtEnd(start))
        {
            return false;
        }

        if (!TryGetCharacterEndPosition(memory, position, out position))
        {
            return false;
        }

        var length = position - start;

        // Special word (e.g. true, false, null, timestamp)
        foreach (var keyword in SpecialValueWords.Where(x => x.Length == length))
        {
            if (memory.EqualsWordIgnoreCase(start, keyword, out _))
            {
                lex = new Lex(memory, LexType.Value, start, position - start);
                endPosition = position;
                return true;
            }
        }

        // next charactor check
        var nextPost = position;
        memory.SkipWhiteSpacesAndComment(nextPost, out nextPost);

        // end of text check
        if (memory.IsAtEnd(nextPost))
        {
            lex = new Lex(memory, LexType.Column, start, position - start);
            endPosition = position;
            return true;
        }

        // left paren
        if (memory.EqualsChar(nextPost, '(', out _))
        {
            lex = new Lex(memory, LexType.Function, start, position - start);
            endPosition = position;
            return true;
        }

        // Identifier separator
        if (memory.EqualsChar(nextPost, '.', out _))
        {
            lex = new Lex(memory, LexType.Namespace, start, position - start);
            endPosition = position;
            return true;
        }

        // other
        lex = new Lex(memory, LexType.Column, start, position - start);
        endPosition = position;
        return true;
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

        if (memory.Span[position].TryGetDbmsValueEscapeChar(out var closeChar))
        {
            // Skip the starting escape symbol
            position++;

            bool isClosed = false;

            // Loop until the end of the identifier or a symbol/whitespace is encountered
            while (!memory.IsAtEnd(position))
            {
                char currentChar = memory.Span[position];

                if (currentChar == closeChar)
                {
                    // If the next character is also a closing escape character, treat it as an escaped character
                    if (!memory.IsAtEnd(position + 1) && memory.Span[position + 1] == closeChar)
                    {
                        position += 2; // Skip both characters
                        continue;
                    }

                    // Otherwise, this is the actual closing escape character
                    isClosed = true;
                    break;
                }

                position++; // Move to the next character
            }

            // If no closing escape character was found, throw an exception
            if (!isClosed)
            {
                throw new FormatException("Unclosed escape character found in the input.");
            }

            // Skip the closing escape symbol
            position++;
        }
        else
        {
            // Loop until the end of the identifier or a symbol/whitespace is encountered.
            while (!memory.IsAtEnd(position))
            {
                var current = memory.Span[position];
                if (current.IsWhiteSpace() || current.IsSymbols())
                {
                    break;
                }
                position++;
            }
        }

        // Return false if no valid identifier was found.
        return (position != start);
    }
}