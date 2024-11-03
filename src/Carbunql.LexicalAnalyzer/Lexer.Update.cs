using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    private static bool TryParseUpdateLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        // Check for "update" keyword
        if (!memory.EqualsWordIgnoreCase(position, "update"))
        {
            return false;
        }

        // Create lex for the update statement
        lex = new Lex(memory, LexType.Update, position, 6);
        return true;
    }
}
