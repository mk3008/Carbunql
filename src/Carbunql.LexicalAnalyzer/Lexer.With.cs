using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    internal static bool TryParseWithOrRecursiveLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if (!memory.EqualsWordIgnoreCase(position, "with"))
        {
            return false;
        }

        // Starting position for the lex and move position past "with"
        var start = position;
        position += 4;
        var withEndPosition = position;

        SkipWhiteSpaces(memory, ref position);

        // Check for "recursive"
        if (memory.EqualsWordIgnoreCase(position, "recursive"))
        {
            lex = new Lex(memory, LexType.WithRecursive, start, position + 9 - start);
            return true;
        }

        lex = new Lex(memory, LexType.With, start, withEndPosition - start);
        return true;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseWithLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;
        if (!memory.EqualsWordIgnoreCase(position, "with"))
        {
            return false;
        }

        lex = new Lex(memory, LexType.With, position, position + 4 - start);
        return true;
    }
}
