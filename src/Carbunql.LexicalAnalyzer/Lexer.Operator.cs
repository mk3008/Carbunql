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

    private static bool TryParseStringOperator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        // Ensure the minimum length of the operator is 2 characters
        if (memory.Length < position + 2)
        {
            return false;
        }

        // Check for "like" operator
        if (memory.EqualsWordIgnoreCase(position, "like"))
        {
            position += 4;
            lex = new Lex(memory, LexType.Operator, start, position - start);
            return true;
        }

        // Check for "and" operator
        if (memory.EqualsWordIgnoreCase(position, "and"))
        {
            position += 3;
            lex = new Lex(memory, LexType.Operator, start, position - start);
            return true;
        }

        // Check for "or" operator
        if (memory.EqualsWordIgnoreCase(position, "or"))
        {
            position += 2;
            lex = new Lex(memory, LexType.Operator, start, position - start);
            return true;
        }

        // Check for "is" operator
        if (memory.EqualsWordIgnoreCase(position, "is"))
        {
            position += 2;

            // Skip whitespace and comments for compound Lex
            SkipWhiteSpacesAndComment(memory, ref position);

            // Check for "not" after "is"
            if (memory.EqualsWordIgnoreCase(position, "not"))
            {
                position += 3;
                lex = new Lex(memory, LexType.Operator, start, position - start, "is not");
            }
            else
            {
                lex = new Lex(memory, LexType.Operator, start, 2);
            }
            return true;
        }

        return false; // Return false if no operator was found
    }
}
