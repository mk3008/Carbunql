using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    private static bool TryParseLeftParen(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        return memory.TryParseSingleCharLex(ref position, '(', LexType.LeftParen, out lex);
    }

    private static Lex ParseRightParen(ReadOnlyMemory<char> memory, ref int position)
    {
        if (memory.TryParseSingleCharLex(ref position, ')', LexType.RightParen, out var lex)) return lex;
        throw new FormatException($"Expected a closing parenthesis ')' at position {position} in the input string.");
    }
}
