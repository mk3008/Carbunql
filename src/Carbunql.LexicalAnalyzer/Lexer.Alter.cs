using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

//public static partial class Lexer
//{
//    [MemberNotNullWhen(true)]
//    private static bool TryParseAlterLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
//    {
//        lex = default;

//        // Check for "alter" keyword
//        if (!memory.EqualsWordIgnoreCase(position, "alter"))
//        {
//            return false;
//        }

//        // Starting position for the lex and move position past "alter"
//        var start = position;
//        position += 6; // Move past "alter"

//        // Check for supported objects after "alter"
//        if (memory.EqualsWordIgnoreCase(position, "table"))
//        {
//            lex = new Lex(memory, LexType.AlterTable, start, position + 5 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "view"))
//        {
//            lex = new Lex(memory, LexType.AlterView, start, position + 4 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "index"))
//        {
//            lex = new Lex(memory, LexType.AlterIndex, start, position + 5 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "schema"))
//        {
//            lex = new Lex(memory, LexType.AlterSchema, start, position + 6 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "function"))
//        {
//            lex = new Lex(memory, LexType.AlterFunction, start, position + 8 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "procedure"))
//        {
//            lex = new Lex(memory, LexType.AlterProcedure, start, position + 9 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "trigger"))
//        {
//            lex = new Lex(memory, LexType.AlterTrigger, start, position + 7 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "sequence"))
//        {
//            lex = new Lex(memory, LexType.AlterSequence, start, position + 8 - start);
//            return true;
//        }

//        throw new NotSupportedException($"Unsupported lex type encountered at position {position}. " +
//                                  $"Expected types: 'table', 'view', 'index', 'schema', 'function', " +
//                                  $" 'procedure', 'trigger', or 'sequence'.");
//    }
//}
