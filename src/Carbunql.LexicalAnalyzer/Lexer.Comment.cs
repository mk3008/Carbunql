using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    internal static void SkipWhiteSpacesAndComment(ReadOnlyMemory<char> memory, ref int position)
    {
        SkipWhiteSpaces(memory, ref position);
        SkipComment(memory, ref position);
        SkipWhiteSpaces(memory, ref position);
    }

    internal static void SkipWhiteSpaces(ReadOnlyMemory<char> memory, ref int position)
    {
        var span = memory.Span;

        while (position < span.Length && char.IsWhiteSpace(span[position]))
        {
            position++;
        }
    }

    internal static void SkipComment(ReadOnlyMemory<char> memory, ref int position)
    {
        if (ParseUntilNonComment(memory, position).Any())
        {
            position = ParseUntilNonComment(memory, position).Last().EndPosition;
        }
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseCommentStartLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        if (TryParseLineCommentStartLex(memory, ref position, out lex)) return true;
        if (TryParseBlockCommentStartLex(memory, ref position, out lex)) return true;
        return false;
    }

    /// <summary>
    /// Parses and removes comments until a non-comment Lex is reached, starting from the specified non-comment state.
    /// </summary>
    /// <param name="memory">The string to be parsed.</param>
    /// <param name="previous">The previous Lex indicating a non-comment state, or null if no previous state exists.</param>
    /// <returns>An enumeration of Lexes after comments have been removed.</returns>
    public static IEnumerable<Lex> ParseUntilNonComment(ReadOnlyMemory<char> memory, Lex? previous = null)
    {
        // Invalid if the previous Lex is in a comment state
        if (previous?.Type == LexType.LineCommentStart
            || previous?.Type == LexType.BlockCommentStart
            || previous?.Type == LexType.HitCommentStart
            || previous?.Type == LexType.Comment)
        {
            throw new InvalidOperationException("Previous Lex must be in a non-comment state.");
        }

        // Start position is 0 if previous is null
        int position = previous?.EndPosition ?? 0;

        return ParseUntilNonComment(memory, position);
    }

    public static IEnumerable<Lex> ParseUntilNonComment(ReadOnlyMemory<char> memory, int position)
    {
        while (true)
        {
            Lex lex;
            if (TryParseLineCommentStartLex(memory, ref position, out lex))
            {
                yield return lex;
                yield return ParseLineComment(memory, ref position);
                SkipWhiteSpaces(memory, ref position);
                continue;
            }
            else if (TryParseBlockCommentStartLex(memory, ref position, out lex))
            {
                yield return lex;
                yield return ParseBlockComment(memory, ref position);
                yield return ParseBlockCommentEndLex(memory, ref position);
                SkipWhiteSpaces(memory, ref position);
                continue;
            }
            break;
        }
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseLineCommentStartLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        // must over 2charactor
        if (memory.Length < position + 1)
        {
            return false;
        }

        var start = position;
        if (memory.Span[position] == '-' && memory.Span[position + 1] == '-')
        {
            position += 2;
            lex = new Lex(memory, LexType.LineCommentStart, start, position - start);
            return true;
        }

        return false;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseBlockCommentStartLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        // must over 2charactor
        if (memory.Length < position + 1)
        {
            return false;
        }

        var start = position;
        // Check for /* or /*+
        if (memory.Span[position] == '/' && memory.Span[position + 1] == '*')
        {
            // Check for /*+
            if (memory.Length < position + 2 && memory.Span[position + 2] == '+')
            {
                position += 3;
                lex = new Lex(memory, LexType.HitCommentStart, start, position - start);
            }
            else
            {
                position += 2;
                lex = new Lex(memory, LexType.BlockCommentStart, start, position - start);
            }
            return true;
        }

        // Not a block comment start
        return false;
    }

    private static Lex ParseLineComment(ReadOnlyMemory<char> memory, ref int position)
    {
        if (memory.Length < position)
        {
            throw new FormatException("The SQL string is empty or in an invalid format.");
        }

        var start = position;

        // exclude line comment end symbol
        while (position < memory.Length)
        {
            char current = memory.Span[position];

            if (current == '\r' || current == '\n')
            {
                return new Lex(memory, LexType.Comment, start, position - start);
            }
            position++;
        }
        return new Lex(memory, LexType.Comment, start, memory.Length - start);
    }

    private static Lex ParseBlockComment(ReadOnlyMemory<char> memory, ref int position)
    {
        // must over 2charactor
        if (memory.Length < position + 1)
        {
            throw new FormatException("The SQL string is empty or in an invalid format.");
        }

        var start = position;

        // Search for the block comment end
        while (position < memory.Length)
        {
            if (memory.Span[position] == '*' && position + 1 < memory.Length && memory.Span[position + 1] == '/')
            {
                {
                    // exclude end symbol
                    return new Lex(memory, LexType.Comment, start, position - start);
                }
            }
            position++;
        }

        throw new FormatException("Block comment not closed properly.");
    }

    [MemberNotNullWhen(true)]
    private static Lex ParseBlockCommentEndLex(ReadOnlyMemory<char> memory, ref int position)
    {
        if (!memory.EqualsWordIgnoreCase(position, "*/"))
        {
            throw new FormatException("Block comment end symbols '*/' not found.");
        }
        var start = position;
        position += 2;
        return new Lex(memory, LexType.BlockCommentEnd, start, position - start);
    }
}
