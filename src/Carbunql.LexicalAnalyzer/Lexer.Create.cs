using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

//public static partial class Lexer
//{
//    [MemberNotNullWhen(true)]
//    private static bool TryParseCreateLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
//    {
//        lex = default;

//        // Check for "create" keyword
//        if (!memory.EqualsWordIgnoreCase(position, "create"))
//        {
//            return false;
//        }

//        // Starting position for the lex and move position past "create"
//        var start = position;
//        position += 6; // Move past "create"

//        // Check for supported objects after "create"
//        if (memory.EqualsWordIgnoreCase(position, "temporary"))
//        {
//            position += 9; // Move past "temporary"
//            memory.SkipWhiteSpaces(ref position);

//            if (memory.EqualsWordIgnoreCase(position, "table"))
//            {
//                lex = new Lex(memory, LexType.CreateTemporaryTable, start, position + 5 - start);
//                return true;
//            }
//            else if (memory.EqualsWordIgnoreCase(position, "view"))
//            {
//                lex = new Lex(memory, LexType.CreateView, start, position + 4 - start);
//                return true;
//            }
//            throw new FormatException("Expected 'table' or 'view' after 'temporary'.");
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "table"))
//        {
//            lex = new Lex(memory, LexType.CreateTable, start, position + 5 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "materialized"))
//        {
//            position += 12; // Move past "materialized"
//            memory.SkipWhiteSpaces(ref position);

//            if (memory.EqualsWordIgnoreCase(position, "view"))
//            {
//                lex = new Lex(memory, LexType.CreateView, start, position + 4 - start);
//                return true;
//            }
//            throw new FormatException("Expected 'view' after 'materialized'.");
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "view"))
//        {
//            lex = new Lex(memory, LexType.CreateView, start, position + 4 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "index"))
//        {
//            lex = new Lex(memory, LexType.CreateIndex, start, position + 5 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "schema"))
//        {
//            lex = new Lex(memory, LexType.CreateSchema, start, position + 6 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "function"))
//        {
//            lex = new Lex(memory, LexType.CreateFunction, start, position + 8 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "procedure"))
//        {
//            lex = new Lex(memory, LexType.CreateProcedure, start, position + 9 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "trigger"))
//        {
//            lex = new Lex(memory, LexType.CreateTrigger, start, position + 7 - start);
//            return true;
//        }
//        else if (memory.EqualsWordIgnoreCase(position, "sequence"))
//        {
//            lex = new Lex(memory, LexType.CreateSequence, start, position + 8 - start);
//            return true;
//        }

//        throw new NotSupportedException($"Unsupported lex type encountered at position {position}. " +
//                                  $"Expected types: 'table', 'view', 'index', 'schema', 'function', " +
//                                  $" 'procedure', 'trigger', or 'sequence'.");
//    }
//}
