namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{


    private static bool TryParseStringOperator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        // Ensure the minimum length of the operator is 2 characters
        if (memory.IsAtEnd(position + +2))
        {
            return false;
        }

        // Check for "like" operator
        if (memory.TryParseKeywordIgnoreCase(ref position, "like", LexType.Operator, out lex))
        {
            return true;
        }

        // Check for "not" operator
        if (memory.TryParseKeywordIgnoreCase(ref position, "not", LexType.Operator, out lex))
        {
            return true;
        }

        // Check for "and" operator
        if (memory.TryParseKeywordIgnoreCase(ref position, "and", LexType.Operator, out lex))
        {
            return true;
        }

        // Check for "or" operator
        if (memory.TryParseKeywordIgnoreCase(ref position, "or", LexType.Operator, out lex))
        {
            return true;
        }

        // Check for "is" operator
        if (memory.EqualsWordIgnoreCase(position, "is", out position))
        {
            memory.SkipWhiteSpacesAndComment(ref position);

            // Check for "is not" operator
            if (memory.EqualsWordIgnoreCase(position, "not", out position))
            {
                // "is not"
                lex = new Lex(memory, LexType.Operator, start, position - start, "is not");
                return true;
            }
            else
            {
                // "is"
                lex = new Lex(memory, LexType.Operator, start, 2);
                return true;
            }
        }

        return false; // Return false if no operator was found
    }
}
