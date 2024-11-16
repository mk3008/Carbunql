using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static class ReadOnlyMemoryExtensions
{
    /// <summary>
    /// Skip whitespace and comments.
    /// </summary>
    /// <param name="memory">The memory to skip.</param>
    /// <param name="position">The current position.</param>
    public static void SkipWhiteSpacesAndComment(this ReadOnlyMemory<char> memory, ref int position)
    {
        memory.SkipWhiteSpaces(ref position);
        memory.SkipComment(ref position);
        memory.SkipWhiteSpaces(ref position);
    }

    /// <summary>
    /// Skip comments.
    /// </summary>
    /// <param name="memory">The memory to skip comments from.</param>
    /// <param name="position">The current position.</param>
    public static void SkipComment(this ReadOnlyMemory<char> memory, ref int position)
    {
        var comments = memory.ParseUntilNonComment(position);
        if (comments.Any())
        {
            position = comments.Last().EndPosition;
        }
    }

    /// <summary>
    /// Parse until a non-comment is reached and yield comments.
    /// </summary>
    /// <param name="memory">The memory to parse.</param>
    /// <param name="position">The starting position.</param>
    /// <returns>Enumerable of parsed comments.</returns>
    public static IEnumerable<Lex> ParseUntilNonComment(this ReadOnlyMemory<char> memory, int position)
    {
        while (true)
        {
            Lex lex;
            if (memory.TryParseLineCommentStart(ref position, out lex))
            {
                yield return lex;
                yield return memory.ParseLineComment(ref position);
                memory.SkipWhiteSpaces(ref position);
                continue;
            }
            else if (memory.TryParseBlockCommentStart(ref position, out lex))
            {
                yield return lex;
                yield return memory.ParseBlockComment(ref position);
                yield return memory.ParseBlockCommentEnd(ref position);
                memory.SkipWhiteSpaces(ref position);
                continue;
            }
            break;
        }
    }

    /// <summary>
    /// Parses the end of a block comment from the specified memory, returning the resulting Lex object.
    /// </summary>
    /// <param name="memory">The memory to process.</param>
    /// <param name="position">The current position in the memory.</param>
    /// <returns>The Lex object representing the block comment end.</returns>
    /// <exception cref="FormatException">Thrown when the block comment end symbols '*/' are not found.</exception>
    public static Lex ParseBlockCommentEnd(this ReadOnlyMemory<char> memory, ref int position)
    {
        var start = position;
        if (memory.StartsWithExact(ref position, "*/"))
        {
            return new Lex(memory, LexType.BlockCommentEnd, start, position - start);
        }
        throw new FormatException("Block comment end symbols '*/' not found.");
    }

    /// <summary>
    /// Parses a block comment from the specified memory, returning the resulting Lex object.
    /// </summary>
    /// <param name="memory">The memory to process.</param>
    /// <param name="position">The current position in the memory.</param>
    /// <returns>The Lex object representing the block comment.</returns>
    /// <exception cref="FormatException">Thrown when the SQL string is empty or in an invalid format.</exception>
    public static Lex ParseBlockComment(this ReadOnlyMemory<char> memory, ref int position)
    {
        // Must be at least 2 characters for block comment start
        if (memory.HasFewerThanChars(position, 2))
        {
            throw new FormatException("The SQL string is empty or in an invalid format.");
        }

        var start = position;

        // Search for the block comment end
        while (position < memory.Length)
        {
            if (memory.Span[position] == '*' && memory.HasChar(position + 1) && memory.Span[position + 1] == '/')
            {
                // Exclude end symbol
                return new Lex(memory, LexType.Comment, start, position - start);
            }
            position++;
        }

        throw new FormatException("Block comment not closed properly.");
    }

    /// <summary>
    /// Parses a line comment from the specified memory, returning the resulting Lex object.
    /// </summary>
    /// <param name="memory">The memory to process.</param>
    /// <param name="position">The current position in the memory.</param>
    /// <returns>The Lex object representing the line comment.</returns>
    /// <exception cref="FormatException">Thrown when the SQL string is empty or in an invalid format.</exception>
    public static Lex ParseLineComment(this ReadOnlyMemory<char> memory, ref int position)
    {
        if (memory.IsAtEnd(position))
        {
            throw new FormatException("The SQL string is empty or in an invalid format.");
        }

        var start = position;

        // Exclude line comment end symbol
        while (!memory.IsAtEnd(position))
        {
            char current = memory.Span[position];

            if (current == '\r' || current == '\n')
            {
                return new Lex(memory, LexType.Comment, start, position - start);
            }
            position++;
        }

        return new Lex(memory, LexType.Comment, start, memory.Length - start);
    }

    /// <summary>
    /// Tries to parse a wildcard character ('*') from the specified memory.
    /// </summary>
    /// <param name="memory">The memory to process.</param>
    /// <param name="position">The current position in the memory.</param>
    /// <param name="lex">The resulting Lex object if parsing is successful.</param>
    /// <returns>True if the wildcard character was successfully parsed; otherwise, false.</returns>
    [MemberNotNullWhen(true)]
    public static bool TryParseWildCard(this ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        return memory.TryParseSingleCharLex(ref position, '*', LexType.WildCard, out lex);
    }

    /// <summary>
    /// Skips over white space characters in the given memory starting from the current position.
    /// </summary>
    /// <param name="memory">The memory to process.</param>
    /// <param name="position">The current position to start skipping from. This will be updated.</param>
    public static void SkipWhiteSpaces(this ReadOnlyMemory<char> memory, ref int position)
    {
        var span = memory.Span;

        while (position < span.Length && span[position].IsWhiteSpace())
        {
            position++;
        }
    }

    [MemberNotNullWhen(true)]
    public static bool TryParseBlockCommentStart(this ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if (memory.HasFewerThanChars(position, 2))
        {
            return false;
        }

        var start = position;
        // Check for /* or /*+
        if (memory.Span[position] == '/' && memory.Span[position + 1] == '*')
        {
            // Check for /*+
            if (memory.HasChar(position + 2) && memory.Span[position + 2] == '+')
            {
                position += 3;
                lex = new Lex(memory, LexType.HitCommentStart, start, position - start);
            }
            else
            {
                position += 2;
                lex = new Lex(memory, LexType.BlockCommentStart, start, position - start);
            }
            return true;
        }
        return false;
    }

    [MemberNotNullWhen(true)]
    public static bool TryParseLineCommentStart(this ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        if (memory.StartsWithExact(ref position, "--"))
        {
            lex = new Lex(memory, LexType.LineCommentStart, start, position - start);
            return true;
        }

        return false;
    }

    [MemberNotNullWhen(true)]
    public static bool TryParseSingleCharLex(this ReadOnlyMemory<char> memory, ref int position, char targetChar, LexType lexType, out Lex lex)
    {
        lex = default;

        // Ensure there are enough characters remaining
        if (memory.HasFewerThanChars(position, 1))
        {
            return false;
        }

        var start = position;
        if (memory.Span[position] == targetChar)
        {
            position++;
            lex = new Lex(memory, lexType, start, position - start);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the specified position is at the end of the memory.
    /// </summary>
    /// <param name="memory">The memory segment to check.</param>
    /// <param name="position">The position to start checking from.</param>
    /// <returns>Returns true if there is no character available from the specified position; otherwise, false.</returns>
    public static bool IsAtEnd(this ReadOnlyMemory<char> memory, int position)
    {
        return HasFewerThanChars(memory, position, 1);
    }

    /// <summary>
    /// Checks if there are fewer than the specified number of characters from the given position.
    /// </summary>
    /// <param name="memory">The memory segment to check.</param>
    /// <param name="position">The position to start checking from.</param>
    /// <param name="requiredLength">The number of characters required.</param>
    /// <returns>Returns true if there are fewer than the required number of characters; otherwise, false.</returns>
    public static bool HasFewerThanChars(this ReadOnlyMemory<char> memory, int position, int requiredLength)
    {
        return position < 0 || position + requiredLength > memory.Length;
    }

    /// <summary>
    /// Checks if at least one character can be accessed from the specified position in memory.
    /// </summary>
    /// <param name="memory">The memory segment to check.</param>
    /// <param name="position">The position to start checking from.</param>
    /// <returns>Returns true if at least one character can be accessed; otherwise, false.</returns>
    public static bool HasChar(this ReadOnlyMemory<char> memory, int position)
    {
        // Check if position is within bounds and at least one character can be accessed
        return position >= 0 && position < memory.Length;
    }

    public static bool TryParseKeywordIgnoreCase(this ReadOnlyMemory<char> memory, ref int position, string keyword, LexType lexType, out Lex lex)
    {
        lex = default;
        var start = position;

        if (memory.EqualsWordIgnoreCase(position, keyword, out position))
        {
            lex = new Lex(memory, lexType, start, keyword.Length);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Compares a substring of the given <see cref="ReadOnlyMemory{char}"/> 
    /// to a specified keyword, ignoring case, and checks if it matches 
    /// as a complete word.
    /// </summary>
    /// <param name="memory">The memory to search within.</param>
    /// <param name="position">The position within the memory to start the comparison.</param>
    /// <param name="keyword">The keyword to compare against.</param>
    /// <returns>
    /// <c>true</c> if the substring matches the keyword as a complete word; 
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool EqualsWordIgnoreCase(this ReadOnlyMemory<char> memory, int position, string keyword, out int endPosition)
    {
        endPosition = position;
        var keyLength = keyword.Length;

        // Not enough characters remaining for a match
        if (memory.IsAtEnd(position + keyLength - 1))
        {
            return false;
        }

        // Check for keyword match
        for (int i = 0; i < keyLength; i++)
        {
            if (char.ToLowerInvariant(memory.Span[position + i]) != char.ToLowerInvariant(keyword[i]))
            {
                return false;
            }
        }

        // Check the character following the keyword
        int nextPosition = position + keyLength;
        if (memory.IsAtEnd(nextPosition))
        {
            endPosition = nextPosition;
            return true;
        }

        // Get the next character
        char nextChar = memory.Span[nextPosition];

        // Check if the next character is a space, or a symbol (comma, dot, left parenthesis, arithmetic operators, etc.)
        if (nextChar.IsWhiteSpace() || nextChar.IsSymbols())
        {
            endPosition = nextPosition;
            return true;
        }

        // Invalid match (not a complete word)
        return false;
    }

    public static bool IsAtEndOrWhiteSpace(this ReadOnlyMemory<char> memory, int position)
    {
        if (HasFewerThanChars(memory, position, 1)) return false;
        return memory.Span[position].IsWhiteSpace();
    }

    public static bool EqualsChar(this ReadOnlyMemory<char> memory, int start, char keyword, out int position)
    {
        position = start;

        // Not enough characters remaining for a match
        if (memory.HasFewerThanChars(start, 1))
        {
            return false;
        }

        return memory.Span[start] == keyword;
    }

    public static bool EqualsWord(this ReadOnlyMemory<char> memory, int start, string keyword, out int position)
    {
        position = start;
        var keyLength = keyword.Length;

        // Not enough characters remaining for a match
        if (memory.HasFewerThanChars(start, keyLength))
        {
            return false;
        }

        // Check for keyword match
        for (int i = 0; i < keyLength; i++)
        {
            if (memory.Span[start + i] != keyword[i])
            {
                return false;
            }
            position++;
        }

        return true;
    }

    /// <summary>
    /// Checks if the memory at the specified position matches the given keyword with a prefix match.
    /// </summary>
    /// <param name="memory">The memory segment to check.</param>
    /// <param name="position">The position to start comparing from.</param>
    /// <param name="keyword">The string to compare against.</param>
    /// <returns>Returns true if the characters from the specified position match the keyword; otherwise, false.</returns>
    public static bool StartsWithExact(this ReadOnlyMemory<char> memory, ref int position, string keyword)
    {
        var keyLength = keyword.Length;

        // Not enough characters remaining for a match
        if (memory.HasFewerThanChars(position, keyLength))
        {
            return false;
        }

        // Check for prefix match with the keyword
        for (int i = 0; i < keyLength; i++)
        {
            var current = memory.Span[position + i];
            if (current != keyword[i])
            {
                return false;
            }
        }

        position += keyLength;
        return true;
    }
}
