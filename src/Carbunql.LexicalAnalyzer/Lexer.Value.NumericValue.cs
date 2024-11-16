using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static HashSet<char> NumericSigns = new HashSet<char> {
        '+',
        '-'
    };

    [MemberNotNullWhen(true)]
    public static bool TryParseNumericValue(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        if (TryGetNumericEndPosition(memory, position, out position))
        {
            lex = new Lex(memory, LexType.Value, start, position - start);

            // Postgres typeconverter
            start = position;
            if (memory.EqualsWord(position, "::", out position))
            {
                //read expression
            }

            return true;
        }
        return false;
    }

    private static bool TryGetNumericEndPosition(ReadOnlyMemory<char> memory, int startPosition, out int position)
    {
        position = startPosition;
        if (memory.IsAtEnd(position))
        {
            return false;
        }

        // check '+', '-' sign
        // +1, -1 のようなLexをNumericと判定するための処理です。
        if (NumericSigns.Contains(memory.Span[position]))
        {
            position++;
            memory.SkipWhiteSpacesAndComment(ref position);
        }

        // 1文字目が数値でなければ、対象外
        if (memory.IsAtEnd(position) || !char.IsDigit(memory.Span[position]))
        {
            return false;
        }

        var hasDecimalPoint = false;

        // Parse digits and optional decimal point
        while (!memory.IsAtEnd(position))
        {
            char current = memory.Span[position];

            if (char.IsDigit(current))
            {
                position++;
                continue;
            }

            if (current == '.')
            {
                if (!hasDecimalPoint)
                {
                    hasDecimalPoint = true;
                    position++;
                    continue;
                }
                throw new FormatException("Multiple decimal points found in numeric value.");
            }

            break;
        }

        return (startPosition != position);
    }
}
