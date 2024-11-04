namespace Carbunql.LexicalAnalyzer;

internal static class ReadOnlyMemoryExtensions
{
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
    public static bool EqualsWordIgnoreCase(this ReadOnlyMemory<char> memory, int position, string keyword)
    {
        var keyLength = keyword.Length;
        // Not enough characters remaining for a match
        if (memory.HasFewerThanChars(position, keyLength))
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
            return true; // End of memory, valid match
        }

        // Get the next character
        char nextChar = memory.Span[nextPosition];

        // Check if the next character is a space, or a symbol (comma, dot, left parenthesis, arithmetic operators, etc.)
        if (char.IsWhiteSpace(nextChar) || char.IsSymbol(nextChar))
        {
            return true; // Valid word match
        }

        // Invalid match (not a complete word)
        return false;
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
