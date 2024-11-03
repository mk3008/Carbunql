namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static IEnumerable<Lex> TryParseSchemaOrTableOrColumns(ReadOnlyMemory<char> memory, int position)
    {
        var start = position;

        // Ensure the first character is a letter
        if (position >= memory.Length || !char.IsLetter(memory.Span[position]))
        {
            yield break;
        }

        // Move past the first character
        position++;

        while (position < memory.Length)
        {
            char currentChar = memory.Span[position];

            // Check for the dot separator
            if (position + 1 < memory.Length && memory.Span[position + 1] == '.')
            {
                // Yield the current identifier without the separator
                yield return new Lex(memory, LexType.SchemaOrTableOrColumn, start, position - start + 1);
                position += 2; // Move past the separator
                start = position; // Update the start for the next identifier
                continue;
            }

            // Terminate on whitespace or disallowed characters
            if (char.IsWhiteSpace(currentChar) || (currentChar != '_' && currentChar != '-' && !char.IsLetterOrDigit(currentChar)))
            {
                yield return new Lex(memory, LexType.SchemaOrTableOrColumn, start, position - start);
                break;
            }

            position++;
        }

        // Yield the final identifier if valid
        if (position > start)
        {
            yield return new Lex(memory, LexType.SchemaOrTableOrColumn, start, position - start);
        }
    }
}