using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    internal static bool TryParseValueLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        if (TryParseWildCard(memory, ref position, out lex)) return true;
        if (TryParseNumericValue(memory, ref position, out lex)) return true;
        if (TryParseSingleQuotedText(memory, ref position, out lex)) return true;
        //if (TryParseLetterValueLex(memory, ref position, out lex)) return true;
        return false;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseWildCard(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        return TryParseSingleCharLex(memory, ref position, '*', LexType.WildCard, out lex);
    }

    [MemberNotNullWhen(true)]
    public static bool TryParseLetter(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if (memory.Length < position + 1)
        {
            return false;
        }

        int start = position;

        while (position < memory.Length)
        {
            char current = memory.Span[position];

            if (char.IsLetter(current) || current == '_')
            {
                position++;
                continue;
            }

            break;
        }

        // If no letter were found
        if (start == position)
        {
            return false;
        }

        //check next lex
        int tempPos = position;
        SkipWhiteSpaces(memory, ref tempPos);

        LexType lexType;
        if (tempPos < memory.Length)
        {
            char nextChar = memory.Span[tempPos];
            if (nextChar == '.')
            {
                lexType = LexType.SchemaOrTableOrColumn;
            }
            else if (nextChar == ',' || char.IsWhiteSpace(nextChar))
            {
                lexType = LexType.Column;
            }
            else if (nextChar == '(')
            {
                lexType = LexType.Function;
            }
            else
            {
                lexType = LexType.Value;
            }
        }
        else
        {
            lexType = LexType.Value;
        }

        lex = new Lex(memory, lexType, start, position - start);
        return true;
    }

    //[MemberNotNullWhen(true)]
    //public static bool TryParseLetterValueLex(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    //{
    //    lex = default;

    //    if (memory.Length < position + 1)
    //    {
    //        return false;
    //    }

    //    int start = position;

    //    while (position < memory.Length)
    //    {
    //        char current = memory.Span[position];

    //        if (char.IsLetter(current) || current == '_')
    //        {
    //            position++;
    //            continue;
    //        }

    //        break;
    //    }

    //    // If no letter were found
    //    if (start == position)
    //    {
    //        return false;
    //    }

    //    //check next lex
    //    int tempPos = position;
    //    SkipWhiteSpaces(memory, ref tempPos);

    //    LexType lexType;
    //    if (tempPos < memory.Length)
    //    {
    //        char nextChar = memory.Span[tempPos];
    //        if (nextChar == '.')
    //        {
    //            lexType = LexType.SchemaOrTableOrColumn;
    //        }
    //        else if (nextChar == ',' || char.IsWhiteSpace(nextChar))
    //        {
    //            lexType = LexType.Column;
    //        }
    //        else if (nextChar == '(')
    //        {
    //            lexType = LexType.Function;
    //        }
    //        else
    //        {
    //            lexType = LexType.Value;
    //        }
    //    }
    //    else
    //    {
    //        lexType = LexType.Value;
    //    }

    //    lex = new Lex(memory, lexType, start, position - start);
    //    return true;
    //}

    [MemberNotNullWhen(true)]
    public static bool TryParseNumericValue(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        // Complete starting position including the optional sign
        var start = position;
        bool hasDecimalPoint = false;

        // Check for optional sign at the beginning
        char firstChar = memory.Span[position];
        if (firstChar == '+' || firstChar == '-')
        {
            position++; // Skip the sign
        }

        // Skip whitespace after the sign
        SkipWhiteSpaces(memory, ref position);

        // Start position for digits (after sign and whitespace)
        var digitStart = position;

        while (position < memory.Length)
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

            break; // Exit loop on encountering a non-digit and non-decimal point
        }

        // If no digits were found
        if (digitStart == position)
        {
            return false;
        }

        // Include the optional sign in the Lex object
        lex = new Lex(memory, LexType.Value, start, position - start);
        return true;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseSingleQuotedText(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;

        if (memory.Span[position] != '\'')
        {
            return false;
        }

        int start = position; // Remember the starting position, including the starting single quote
        position++; // Skip the starting single quote

        while (position < memory.Length)
        {
            char current = memory.Span[position];

            if (current == '\'')
            {
                // Check for escape sequence
                if (position + 1 < memory.Length && memory.Span[position + 1] == '\'')
                {
                    position += 2; // Skip the escaped single quote (2 characters)
                    continue;
                }

                position++; // Include the ending single quote
                lex = new Lex(memory, LexType.Value, start, position - start);
                return true;
            }

            position++;
        }

        throw new FormatException("Unterminated string literal in SQL.");
    }
}