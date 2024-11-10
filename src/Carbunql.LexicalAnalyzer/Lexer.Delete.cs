using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

//public static partial class Lexer
//{
//    [MemberNotNullWhen(true)]
//    private static bool TryParseDeleteLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
//    {
//        lex = default;

//        // Check for "delete" keyword
//        if (!memory.EqualsWordIgnoreCase(position, "delete"))
//        {
//            return false;
//        }

//        // Starting position for the lex and move position past "delete"
//        var start = position;
//        position += 6; // Move past "delete"
//        memory.SkipWhiteSpaces(ref position); // Skip any whitespace after "delete"

//        // Check for "from"
//        if (!memory.EqualsWordIgnoreCase(position, "from"))
//        {
//            throw new FormatException("Expected 'from' after 'delete'.");
//        }

//        // Create lex for the delete statement
//        lex = new Lex(memory, LexType.Delete, start, position + 4 - start);
//        return true;
//    }
//}

