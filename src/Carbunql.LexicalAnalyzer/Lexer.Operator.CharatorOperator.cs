namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    internal static bool TryParseOperator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        if (TryParseCharOperator(memory, position, out lex, out position)) return true;
        if (TryParseStringOperator(memory, position, out lex, out position)) return true;
        return false;
    }

    private static readonly HashSet<char> Operators = new HashSet<char> { '*', '+', '-', '/', '=', '<', '>', '!', '^', ':' };

    private static bool TryParseCharOperator(ReadOnlyMemory<char> memory, int start, out Lex lex, out int endPosition)
    {
        lex = default;
        var pos = start;
        endPosition = start;

        // Ensure we are within bounds
        if (memory.IsAtEnd(pos))
        {
            return false;
        }

        // Logic assumes comment-starting Lex is removed
        // Check for consecutive symbol characters to identify an operator
        while (!memory.IsAtEnd(pos) && Operators.Contains(memory.Span[pos]))
        {
            pos++;
        }

        // If no symbols were found, return false
        if (pos == start)
        {
            return false;
        }

        // Successfully parsed an operator
        endPosition = pos;
        lex = new Lex(memory, LexType.Operator, start, endPosition - start);
        return true;
    }
}
