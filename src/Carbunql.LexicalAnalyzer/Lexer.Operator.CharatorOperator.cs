namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    internal static bool TryParseOperator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        if (TryParseCharOperator(memory, ref position, out lex)) return true;
        if (TryParseStringOperator(memory, ref position, out lex)) return true;
        return false;
    }

    private static readonly HashSet<char> Operators = new HashSet<char> { '*', '+', '-', '/', '=', '<', '>', '!', '^' };

    private static bool TryParseCharOperator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        // Ensure we are within bounds
        if (memory.IsAtEnd(position))
        {
            return false;
        }

        // Logic assumes comment-starting Lex is removed
        // Check for consecutive symbol characters to identify an operator
        var start = position;
        while (!memory.IsAtEnd(position) && Operators.Contains(memory.Span[position]))
        {
            position++;
        }

        // If no symbols were found, return false
        if (start == position)
        {
            return false;
        }

        // Successfully parsed an operator
        lex = new Lex(memory, LexType.Operator, start, position - start);
        return true;
    }
}
