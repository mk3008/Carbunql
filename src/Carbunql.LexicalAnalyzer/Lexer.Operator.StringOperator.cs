namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static HashSet<string> StringOperators = new HashSet<string>()
    {
        "like",
        "not",
        "and",
        "or",
        "as", //type cast "as"
    };

    private static bool TryParseStringOperator(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        // Ensure the minimum length of the operator is 2 characters
        if (memory.IsAtEnd(position + +2))
        {
            return false;
        }

        foreach (var item in StringOperators)
        {
            if (memory.TryParseKeywordIgnoreCase(ref position, item, LexType.Operator, out lex))
            {
                return true;
            }
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
