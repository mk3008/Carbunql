using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    public static Lex ParseFrom(ReadOnlyMemory<char> memory, int start, out int endPosition)
    {
        endPosition = start;
        var pos = start;

        //memory.SkipWhiteSpacesAndComment(pos, out pos);

        if (memory.TryParseKeywordIgnoreCase(pos, "from", LexType.From, out var lex, out pos))
        {
            endPosition = lex.EndPosition;
            return lex;
        }
        throw new FormatException();
    }

    public static bool TryParseExpressionSeparator(ReadOnlyMemory<char> memory, int start, out Lex lex, out int endPosition)
    {
        endPosition = start;
        var pos = start;

        //memory.SkipWhiteSpacesAndComment(pos, out pos);

        if (memory.TryParseSingleCharLex(pos, ',', LexType.ExpressionSeparator, out lex, out pos))
        {
            endPosition = lex.EndPosition;
            return true;
        }

#if DEBUG
        if (memory.IsAtEnd(pos))
        {
            Debug.Print($"position:{pos}, value:EOF");
        }
        else
        {
            Debug.Print($"position:{pos}, value:{memory.Span[pos]}");
        }
#endif
        return false;
    }
}
