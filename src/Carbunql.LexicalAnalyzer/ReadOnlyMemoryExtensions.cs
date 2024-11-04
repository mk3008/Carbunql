namespace Carbunql.LexicalAnalyzer;

internal static class ReadOnlyMemoryExtensions
{
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
        // Not enough characters remaining for a match
        if (position + keyword.Length > memory.Length)
        {
            return false;
        }

        // Check for keyword match
        for (int i = 0; i < keyword.Length; i++)
        {
            if (char.ToLowerInvariant(memory.Span[position + i]) != char.ToLowerInvariant(keyword[i]))
            {
                // Mismatch found
                return false;
            }
        }

        // Check the character following the keyword
        int nextPosition = position + keyword.Length;

        // If nextPosition is at the end of the memory, it's a valid match
        if (nextPosition == memory.Length - 1)
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
}
