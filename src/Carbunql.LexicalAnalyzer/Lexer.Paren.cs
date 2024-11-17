using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    private static bool TryParseLeftParen(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        return memory.TryParseSingleCharLex(position, '(', LexType.LeftParen, out lex, out position);
    }

    private static Lex ParseLeftParen(ReadOnlyMemory<char> memory, ref int position)
    {
        if (memory.TryParseSingleCharLex(position, '(', LexType.LeftParen, out var lex, out position)) return lex;
        throw new InvalidProgramException();
    }

    private static Lex ParseRightParen(ReadOnlyMemory<char> memory, ref int position)
    {
        if (memory.TryParseSingleCharLex(position, ')', LexType.RightParen, out var lex, out position)) return lex;
        throw new FormatException($"Expected a closing parenthesis ')' at position {position} in the input string.");
    }
}
