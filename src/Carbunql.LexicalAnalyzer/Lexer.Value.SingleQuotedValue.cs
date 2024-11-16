using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    private static bool TryParseSingleQuotedValue(ReadOnlyMemory<char> memory, ref int start, out Lex lex)
    {
        lex = default;
        var pos = start;

        if (TryGetSingleQuoteEndPosition(memory, pos, out pos))
        {
            lex = new Lex(memory, LexType.Value, start, pos - start);
            return true;
        }
        return false;
    }

    private static bool TryGetSingleQuoteEndPosition(ReadOnlyMemory<char> memory, int start, out int position)
    {
        position = start;
        if (memory.IsAtEnd(position))
        {
            return false;
        }
        if (memory.Span[position] != '\'')
        {
            return false;
        }
        position++;

        while (!memory.IsAtEnd(position))
        {
            char current = memory.Span[position];

            if (current == '\'')
            {
                // Check for escape sequence
                if (!memory.IsAtEnd(position + 1) && memory.Span[position + 1] == '\'')
                {
                    // Skip the escaped single quote (2 characters)
                    position += 2;
                    continue;
                }

                position++; // Include the ending single quote
                return true;
            }

            position++;
        }

        throw new FormatException("シングルコーテーションが閉じられていません");
    }
}
