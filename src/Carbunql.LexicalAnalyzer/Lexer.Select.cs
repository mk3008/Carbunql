using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    internal static bool TryParseSelect(ReadOnlyMemory<char> memory, int start, out Lex lex, out int endPosition)
    {
        endPosition = start;
        lex = default;
        var pos = start;

        // Check for "select" keyword
        if (!memory.EqualsWordIgnoreCase(pos, "select", out pos))
        {
            return false;
        }

        memory.SkipWhiteSpacesAndComment(pos, out pos);

        // Check for "all"
        if (memory.EqualsWordIgnoreCase(pos, "all", out pos))
        {
            lex = new Lex(memory, LexType.Select, pos, pos - start, "select all");
            endPosition = lex.EndPosition;
            return true;
        }

        // Check for "distinct"
        if (memory.EqualsWordIgnoreCase(pos, "distinct", out pos))
        {
            memory.SkipWhiteSpacesAndComment(pos, out pos);

            if (memory.EqualsWordIgnoreCase(pos, "on", out pos))
            {
                lex = new Lex(memory, LexType.Select, pos, pos - start, "select distinct on");
                endPosition = lex.EndPosition;
                return true;
            }

            lex = new Lex(memory, LexType.Select, pos, pos - start, "select distinct");
            endPosition = lex.EndPosition;
            return true;
        }

        // If neither "all" nor "distinct", return simple "select" lex
        lex = new Lex(memory, LexType.Select, start, pos - start, "select");
        endPosition = lex.EndPosition;
        return true;
    }

    //[MemberNotNullWhen(true)]
    //internal static bool TryParseAliasKeyword(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    //{
    //    lex = default;

    //    if (!memory.EqualsWordIgnoreCase(position, "as"))
    //    {
    //        return false;
    //    }

    //    lex = new Lex(memory, LexType.As, position, 2);
    //    return true;
    //}
}
