using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

//public static partial class Lexer
//{
//    [MemberNotNullWhen(true)]
//    private static bool TryParseInsertLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
//    {
//        lex = default;

//        // Check for "insert" keyword
//        if (!memory.EqualsWordIgnoreCase(position, "insert"))
//        {
//            return false;
//        }

//        // Starting position for the lex and move position past "insert"
//        var start = position;
//        position += 6;
//        memory.SkipWhiteSpaces(ref start);

//        // Check for "into"
//        if (!memory.EqualsWordIgnoreCase(position, "into"))
//        {
//            throw new FormatException("Expected 'into' after 'insert'.");
//        }

//        lex = new Lex(memory, LexType.Insert, start, position + 4 - start);
//        return true;
//    }
//}
