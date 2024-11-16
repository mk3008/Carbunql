using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    internal static bool TryParseSelect(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        // Check for "select" keyword
        if (!memory.EqualsWordIgnoreCase(position, "select", out position))
        {
            return false;
        }

        memory.SkipWhiteSpacesAndComment(ref position);

        // Check for "all"
        if (memory.EqualsWordIgnoreCase(position, "all", out position))
        {
            lex = new Lex(memory, LexType.Select, position, position - start, "select all");
            return true;
        }

        // Check for "distinct"
        if (memory.EqualsWordIgnoreCase(position, "distinct", out position))
        {
            memory.SkipWhiteSpacesAndComment(ref position);
            if (memory.EqualsWordIgnoreCase(position, "on", out position))
            {
                lex = new Lex(memory, LexType.Select, position, position - start, "select distinct on");
                return true;
            }
            lex = new Lex(memory, LexType.Select, position, position - start, "select distinct");
            return true;
        }

        // If neither "all" nor "distinct", return simple "select" lex
        lex = new Lex(memory, LexType.Select, start, position - start, "select");
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
