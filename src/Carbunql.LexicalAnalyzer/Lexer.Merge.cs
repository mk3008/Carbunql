using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    private static bool TryParseMergeLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        // Check for "merge" keyword
        if (!memory.EqualsWordIgnoreCase(position, "merge"))
        {
            return false;
        }

        // Starting position for the lex and move position past "merge"
        var start = position;
        position += 5; // Move past "merge"
        memory.SkipWhiteSpaces(ref position); // Skip any whitespace after "merge"

        // Check for "into"
        if (!memory.EqualsWordIgnoreCase(position, "into"))
        {
            throw new FormatException("Expected 'into' after 'merge'.");
        }

        // Create lex for the merge statement
        lex = new Lex(memory, LexType.Merge, start, position + 4 - start);
        return true;
    }
}
