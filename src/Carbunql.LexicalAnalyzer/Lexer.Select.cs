using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    internal static bool TryParseSelectLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        // Check for "select" keyword
        if (!memory.EqualsWordIgnoreCase(position, "select"))
        {
            return false;
        }

        // Starting position for the lex and move position past "select"
        var start = position;
        position += 6;

        // Check for "all"
        if (memory.EqualsWordIgnoreCase(position, "all"))
        {
            lex = new Lex(memory, LexType.Select, start, position + 3 - start);
            return true;
        }

        // Check for "distinct"
        if (!memory.EqualsWordIgnoreCase(position, "distinct"))
        {
            // If neither "all" nor "distinct", return simple "select" lex
            lex = new Lex(memory, LexType.Select, start, position - start);
            return true;
        }

        // Move position past "distinct"
        position += 8; // Move past "distinct"
        var distinctEndPosition = position; // End position for distinct lex

        SkipWhiteSpaces(memory, ref position); // Skip any whitespace after "distinct"

        // Check Postgres syntax "select distinct on"
        if (memory.EqualsWordIgnoreCase(position, "on"))
        {
            lex = new Lex(memory, LexType.SelectDistinctOn, start, position + 2 - start);
            return true;
        }

        lex = new Lex(memory, LexType.SelectDistinct, start, distinctEndPosition - start);
        return true;
    }

    [MemberNotNullWhen(true)]
    internal static bool TryParseAliasKeyword(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if (!memory.EqualsWordIgnoreCase(position, "as"))
        {
            return false;
        }

        lex = new Lex(memory, LexType.As, position, 2);
        return true;
    }
}
