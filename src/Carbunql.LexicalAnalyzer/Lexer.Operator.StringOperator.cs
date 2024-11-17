namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static HashSet<string> StringOperators = new HashSet<string>()
    {
        "like",
        "not",
        "and",
        "or",
        //"as", //type cast "as"
    };

    private static bool TryParseStringOperator(ReadOnlyMemory<char> memory, int start, out Lex lex, out int endPosition)
    {
        lex = default;
        endPosition = start;
        var pos = start;

        // Ensure the minimum length of the operator is 2 characters
        if (memory.IsAtEnd(pos + 2))
        {
            return false;
        }

        foreach (var item in StringOperators)
        {
            if (memory.TryParseKeywordIgnoreCase(pos, item, LexType.Operator, out lex, out pos))
            {
                endPosition = lex.EndPosition;
                return true;
            }
        }

        // Check for "is" operator
        if (memory.EqualsWordIgnoreCase(pos, "is", out pos))
        {
            memory.SkipWhiteSpacesAndComment(pos, out pos);

            // Check for "is not" operator
            if (memory.EqualsWordIgnoreCase(pos, "not", out pos))
            {
                // "is not"
                lex = new Lex(memory, LexType.Operator, start, pos - start, "is not");
                endPosition = lex.EndPosition;
                return true;
            }
            else
            {
                // "is"
                lex = new Lex(memory, LexType.Operator, start, 2);
                endPosition = lex.EndPosition;
                return true;
            }
        }

        return false; // Return false if no operator was found
    }
}
