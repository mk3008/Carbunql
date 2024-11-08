using System.Diagnostics.CodeAnalysis;

namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    [MemberNotNullWhen(true)]
    internal static bool TryParseNamespaceOrColumn(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        if (TryParseSchemaOrTableOrColumnAsDefault(memory, ref position, out lex)) return true;
        if (TryParseSchemaOrTableOrColumnAsWithDelimiters(memory, ref position, '"', '"', out lex)) return true; // Postgres
        if (TryParseSchemaOrTableOrColumnAsWithDelimiters(memory, ref position, '[', ']', out lex)) return true; // SQLServer
        if (TryParseSchemaOrTableOrColumnAsWithDelimiters(memory, ref position, '`', '`', out lex)) return true; // MySQL

        return false;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseSchemaOrTableOrColumnAsDefault(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        if (memory.IsAtEnd(position))
        {
            return false;
        }

        // Ensure the first character is a letter or "_"
        if (!(char.IsLetter(memory.Span[position]) || memory.Span[position] == '_'))
        {
            return false;
        }

        var backup = position;

        while (!memory.IsAtEnd(position))
        {
            char next = memory.Span[position];

            // Allow letters, digits, underscores
            if (char.IsLetterOrDigit(next) || next == '_')
            {
                position++;
                continue;
            }

            // Check for the dot separator
            if (next == '.')
            {
                lex = new Lex(memory, LexType.Namespace, start, position - start);
                return true; // Successfully parsed schema or table
            }

            // If it's any other character, we terminate the parsing here
            break;
        }

        // Yield the final identifier if valid
        if (position > start)
        {
            var name = memory.Slice(start, position - start).ToString();
            if (name.ToLowerInvariant() == "from")
            {
                //rollback and exit;
                position = backup;
                return false;
            }

            lex = new Lex(memory, LexType.Column, start, position - start, name);
            return true;
        }

        return false;
    }

    [MemberNotNullWhen(true)]
    private static bool TryParseSchemaOrTableOrColumnAsWithDelimiters(ReadOnlyMemory<char> memory, ref int position, char openingDelimiter, char closingDelimiter, out Lex lex)
    {
        lex = default;
        var start = position;

        // Ensure the first character is the opening delimiter
        if (position >= memory.Length || memory.Span[position] != openingDelimiter)
        {
            return false; // Not a valid start for a quoted identifier
        }

        position++; // Move past the opening delimiter

        while (position < memory.Length)
        {
            char next = memory.Span[position];

            // Allow letters, digits, underscores, dashes, or whitespace
            if (char.IsLetterOrDigit(next) || next == '_' || next == '-' || char.IsWhiteSpace(next))
            {
                position++;
                continue; // Continue parsing valid characters
            }

            // Check for the closing delimiter
            if (next == closingDelimiter)
            {
                position++; // Move past the closing delimiter

                // Check if the next character is a dot
                if (position < memory.Length && memory.Span[position] == '.')
                {
                    lex = new Lex(memory, LexType.Namespace, start, position - start);
                    return true; // Successfully parsed schema or table
                }
                else
                {
                    lex = new Lex(memory, LexType.Column, start, position - start);
                    return true; // Successfully parsed column
                }
            }

            // If it's any other character, terminate the parsing here
            break;
        }

        return false; // No valid identifier found
    }


    internal static IEnumerable<Lex> ParseSchemaOrTableOrColumns(ReadOnlyMemory<char> memory, int position)
    {
        var start = position;

        // Ensure the first character is a letter
        if (position >= memory.Length || !char.IsLetter(memory.Span[position]))
        {
            yield break;
        }

        // Move past the first character
        position++;

        while (position < memory.Length)
        {
            char currentChar = memory.Span[position];

            // Check for the dot separator
            if (position + 1 < memory.Length && memory.Span[position + 1] == '.')
            {
                // Yield the current identifier without the separator
                yield return new Lex(memory, LexType.SchemaOrTableOrColumn, start, position - start + 1);
                position += 2; // Move past the separator
                start = position; // Update the start for the next identifier
                continue;
            }

            // Terminate on whitespace or disallowed characters
            if (char.IsWhiteSpace(currentChar) || (currentChar != '_' && currentChar != '-' && !char.IsLetterOrDigit(currentChar)))
            {
                yield return new Lex(memory, LexType.SchemaOrTableOrColumn, start, position - start);
                break;
            }

            position++;
        }

        // Yield the final identifier if valid
        if (position > start)
        {
            yield return new Lex(memory, LexType.SchemaOrTableOrColumn, start, position - start);
        }
    }
}