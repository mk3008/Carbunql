﻿using Cysharp.Text;
using System.Diagnostics.CodeAnalysis;

namespace Carbunql.Analysis;

/// <summary>
/// Class for reading text on a Lexeme (Lex) basis.
/// Reading can only be performed in the forward direction and cannot be reversed.
/// </summary>
public abstract class LexReader
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="text">The text to be read.</param>
    public LexReader(string text)
    {
        Text = text;
        TextLength = Text.Length;
    }

    private int Index { get; set; } = 0;

    private string Text { get; init; }

    private int TextLength { get; init; }

    public bool TryPeek(char expect)
    {
        if (TryPeekChar(0, out var c) && c == expect)
        {
            return true;
        }
        return false;
    }

    private bool TryPeekChar([MaybeNullWhen(false)] out char c)
    {
        return TryPeekChar(0, out c);
    }

    private bool TryPeekChar(int shift, [MaybeNullWhen(false)] out char c)
    {
        c = default;
        var s = Peek(shift, 1);
        if (!string.IsNullOrEmpty(s))
        {
            c = s.First();
            return true;
        }
        return false;
    }

    private string Peek(int shift, int length)
    {
        if (TextLength == 0) return string.Empty;

        var len = Math.Min(TextLength - Index - shift, length);
        if (len <= 0) return string.Empty;

        return Text.Substring(Index + shift, len);
    }

    private string Peek(int length)
    {
        return Peek(0, length);
    }

    /// <summary>
    /// Reads characters using the CharReader and retrieves Lexemes.
    /// Since this function directly reads the text, peeking is not possible.
    /// It is recommended to obtain Lexemes through the Peek or Read functions.
    /// </summary>
    /// <returns>The retrieved Lexeme.</returns>
    protected virtual string ReadLex()
    {
        SkipSpace();

        if (!TryPeekChar(out var fc))
        {
            return string.Empty;
        }

        // escape identifer
        if (fc == '\"' || fc == '`')
        {
            // for Postgres, MySql
            // ex. "column name", `column name`
            return Read(1) + ReadUntilTerminator(fc);
        }
        else if (fc == '[')
        {
            // for SQLServer
            // ex. [column name]
            return Read(1) + ReadUntilTerminator(']');
        }

        // ex. 'text'
        if (fc == '\'')
        {
            return Read(1) + ReadUntilTerminator(fc);
        }

        // ex. | or ||
        if (fc == '|')
        {
            var lex = string.Empty;
            if (TryRead("||", out lex))
            {
                return lex;
            }
            return Read(1);
        }

        // ex. - or -- or -> or ->>
        if (fc == '-')
        {
            var lex = string.Empty;

            if (TryRead("->>", out lex))
            {
                return lex;
            }

            if (TryRead("->", out lex))
            {
                return lex;
            }

            if (TryRead("--", out lex))
            {
                return lex;
            }
            return Read(1);
        }

        // ex. # or #> or #>>
        if (fc == '#')
        {
            var lex = string.Empty;

            if (TryRead("#>>", out lex))
            {
                return lex;
            }

            if (TryRead("#>", out lex))
            {
                return lex;
            }
            return Read(1);
        }

        // ex. / or /*
        if (fc == '/')
        {
            var lex = string.Empty;
            if (TryRead("/*", out lex))
            {
                return lex;
            }
            return Read(1);
        }

        // ex. * or */
        if (fc == '*')
        {
            var lex = string.Empty;
            if (TryRead("*/", out lex))
            {
                return lex;
            }
            return Read(1);
        }

        // ex. :: (Postgres cast symbol)
        if (fc == ':')
        {
            var lex = string.Empty;
            if (TryRead("::", out lex))
            {
                return lex;
            }
            // continue
        }

        // ForceBreakSymbols
        if (".,();".Contains(fc))
        {
            return Read(1);
        }

        // BitwiseOperatorSymbols
        if ("&|^#~".Contains(fc))
        {
            return Read(1);
        }

        // ex. 123.45
        if ("0123456789".Contains(fc))
        {
            return ReadWhile("0123456789.");
        }

        // operator
        if ("+-*/%<>!=".Contains(fc))
        {
            return ReadWhile("+-*/%<>!=?:@.&|^#~");
        }

        using var sb = ZString.CreateStringBuilder();
        sb.Append(Read(1));

        if (fc == '$')
        {
            // '${'
            if (Peek(1) == "{")
            {
                //Doller varibale. ex.${variable}'
                sb.Append(Read(1));
                sb.Append(ReadUntilAnyChar("}", includeTerminateChar: true));
                return sb.ToString();
            }
            else
            {
                //Postgres doller. ex.'$tag$'
                sb.Append(ReadUntilAnyChar("$", includeTerminateChar: true));
                var tag = sb.ToString();
                var text = ReadUntilWord(tag, includeTerminateChar: true);
                sb.Append(text);
                return sb.ToString();
            }
        }

        // ex. @@ (MySQL system variable prefix)
        if (fc == '@')
        {
            // '@@'
            if (Peek(1) == "@")
            {
                sb.Append(Read(1));
            }
        }

        //The symbol "&" is sometimes used as a Postgres literal string prefix, so it will not break if detected.
        sb.Append(ReadUntilAnyChar("+-*/%<>!=?:@.|^#~ \r\n\t.,();[]|"));

        return sb.ToString();
    }

    /// <summary>
    /// Method to read a single Lexeme.
    /// </summary>
    /// <returns>The read Lexeme.</returns>
    private string Read(int length)
    {
        var s = Peek(length);
        Index += length;
        return s;
    }

    /// <summary>
    /// Method to read a Lexeme, but an exception will be thrown if it does not match the expected value.
    /// </summary>
    /// <param name="expect">The expected Lexeme.</param>
    /// <returns>The read Lexeme.</returns>
    /// <exception cref="Exception">Thrown when the actual Lexeme does not match the expected value.</exception>
    public string Read(string expect)
    {
        SkipSpace();

        var length = expect.Length;
        var actual = Peek(length);

        if (TryMatch(actual, expect, out var lex))
        {
            Index += actual.Length;
            return lex;
        }

        throw new Exception($"expect : '{expect}', actual : '{actual.TrimEnd()}'");
    }

    /// <summary>
    /// Method to read a Lexeme. However, an exception is thrown if the read Lexeme does not match any of the expected values in the list.
    /// </summary>
    /// <param name="expects">List of expected Lexemes.</param>
    /// <returns>The read Lexeme.</returns>
    /// <exception cref="Exception">Thrown when the actual Lexeme does not match any of the expected values.</exception>
    public string Read(IEnumerable<string> expects)
    {
        SkipSpace();

        var length = expects.Max(x => x.Length);
        var actual = Peek(length);

        if (TryMatch(actual, expects, out var lex))
        {
            Index += lex.Length;
            return lex;
        }

        //debug
        throw new Exception($"expect : '{string.Join(",", expects)}', actual : '{actual.TrimEnd()}'");
    }

    /// <summary>
    /// Attempts to read the specified Lexeme if it matches the expected value.
    /// </summary>
    /// <param name="expect">The expected Lexeme.</param>
    /// <param name="lex">The read Lexeme.</param>
    /// <returns>True if a match is found, false otherwise.</returns>
    public bool TryRead(string expect, [MaybeNullWhen(false)] out string lex)
    {
        return TryRead(new[] { expect }, out lex);
    }

    /// <summary>
    /// Attempts to read a Lexeme if it matches any of the expected values in the provided list.
    /// </summary>
    /// <param name="expects">List of expected Lexemes.</param>
    /// <param name="lex">The read Lexeme.</param>
    /// <returns>True if a match is found, false otherwise.</returns>
    public bool TryRead(IEnumerable<string> expects, [MaybeNullWhen(false)] out string lex)
    {
        lex = default;

        SkipSpace();

        var length = expects.Max(x => x.Length);
        var actual = Peek(length);

        if (TryMatch(actual, expects, out lex))
        {
            Index += lex.Length;
            return true;
        }
        return false;
    }

    public string Read() => Reads().First();

    /// <summary>
    /// Method to read Lexemes. Enumerates the read Lexemes.
    /// </summary>
    /// <returns>Enumerable of the read Lexemes.</returns>
    public IEnumerable<string> Reads()
    {
        var w = ReadLex();
        if (string.IsNullOrEmpty(w))
        {
            yield return string.Empty;
            yield break;
        }
        while (!string.IsNullOrEmpty(w))
        {
            yield return w;
            w = ReadLex();
        }
    }

    private void SkipSpace()
    {
        var s = Peek(1);
        while ((s == " " || s == "\r" || s == "\n" || s == "\t"))
        {
            Index++;
            s = Peek(1);
        }
    }

    /// <summary>
    /// Reads until the end of the line. Primarily used for obtaining line comments.
    /// </summary>
    /// <returns>The read string.</returns>
    protected string ReadUntilLineEnd()
    {
        var shift = 0;

        var next = Peek(shift, 1);

        //Terminates when a newline code appears or when it reads to the end
        while (!(next == "\r" || next == "\n" || string.IsNullOrEmpty(next)))
        {
            shift++;
            next = Peek(shift, 1);
        }

        var lex = Read(shift);
        Index++;

        // Processing when the line feed code is "\r\n"
        if (next == "\r" && Peek(shift + 1, 1) == "\n")
        {
            Index++;
        }

        return lex;
    }

    private int FindIndexOfSingleQuoteEnd(int shift)
    {
        return FindIndexOfTerminator(shift, '\'');
    }

    private int FindIndexOfTerminator(int shift, char terminatorSymbol)
    {
        var terminator = terminatorSymbol.ToString();
        var s = Peek(shift, 1);
        while (!string.IsNullOrEmpty(s))
        {
            if (s != terminator)
            {
                shift++;
                s = Peek(shift, 1);
                continue;
            }

            // If the terminator symbol appears consecutively,
            // it is not considered.
            if (s == terminator && Peek(shift + 1, 1) == terminator)
            {
                shift += 2;
                s = Peek(shift, 1);
                continue;
            }

            break;
        }

        if (s != terminator) throw new SyntaxException($"Terminator not found. (\"{terminator}\")");

        shift++;
        return shift;
    }

    private string ReadUntilSingleQuote()
    {
        return ReadUntilTerminator('\'');
    }

    private string ReadUntilTerminator(char terminatorSymbol)
    {
        var shift = FindIndexOfTerminator(0, terminatorSymbol);
        return Read(shift);
    }

    private string ReadWhile(string charArrayString)
    {
        var shift = 0;

        var current = Peek(shift, 1);
        while (!string.IsNullOrEmpty(current) && charArrayString.Contains(current.First()))
        {
            var next = Peek(shift + 1, 1);
            // If a comment start symbol is found, force a stop.
            if ((current == "-" && next == "-") || (current == "/" && next == "*"))
            {
                break;
            }
            shift++;
            current = next;
        }

        if (shift == 0) return string.Empty;
        return Read(shift);
    }

    /// <summary>
    /// Reads characters from the input buffer until any character from the specified set of termination characters is encountered, or until the end of the input buffer is reached.
    /// </summary>
    /// <param name="terminateChars">The set of characters that, if encountered, will terminate the reading process.</param>
    /// <param name="includeTerminateChar">Flag indicating whether the terminating character should be included in the returned string.</param>
    /// <returns>A string containing the characters read from the input buffer.</returns>
    private string ReadUntilAnyChar(IEnumerable<char> terminateChars, bool includeTerminateChar = false)
    {
        var shift = 0;

        var s = Peek(shift, 1);
        while (!string.IsNullOrEmpty(s) && !terminateChars.Contains(s.First()))
        {
            shift++;
            if (s == "'")
            {
                // If a single quote is encountered, the terminating character array is ignored.
                // Force reading until the end of the single quote.
                shift = FindIndexOfSingleQuoteEnd(shift);
            }
            s = Peek(shift, 1);
        }

        if (shift == 0) return string.Empty;
        if (includeTerminateChar) shift++;
        return Read(shift);
    }

    /// <summary>
    /// Reads characters from the input buffer until the specified termination word is encountered, or until the end of the input buffer is reached.
    /// </summary>
    /// <param name="terminateWord">The word that, if encountered, will terminate the reading process.</param>
    /// <param name="includeTerminateChar">Flag indicating whether the termination word should be included in the returned string.</param>
    /// <returns>A string containing the characters read from the input buffer.</returns>
    private string ReadUntilWord(string terminateWord, bool includeTerminateChar = false)
    {
        var shift = 0;
        var length = terminateWord.Length;

        var s = Peek(shift, length);
        while (!string.IsNullOrEmpty(s) && s != terminateWord)
        {
            shift++;
            s = Peek(shift, length);
        }

        if (shift == 0) return string.Empty;
        if (includeTerminateChar) shift += length;

        return Read(shift);
    }

    private bool TryMatch(string actualLex, IEnumerable<string> expectLexs, [MaybeNullWhen(false)] out string lex)
    {
        lex = default;

        foreach (var expectLex in expectLexs)
        {
            if (TryMatch(actualLex, expectLex, out lex))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryMatch(string actualLex, string expectLex, [MaybeNullWhen(false)] out string lex)
    {
        lex = default;

        var specials = new[] { "--", "/*", "::", "@@" };
        var symbols = " \r\r\n\t.,();[]'";
        var numbers = "0123456789";

        if (actualLex.StartsWith(expectLex, StringComparison.OrdinalIgnoreCase))
        {
            //NOTE
            //It is necessary to evaluate whether the string simply obtained
            //without passing through ReadLex is a lexeme.

            //Adopt if it exactly matches a special lexeme
            if (specials.Contains(actualLex))
            {
                lex = expectLex;
                return true;
            }

            //Verify next occurrence of character
            var c = Peek(expectLex.Length, 1);
            if (string.IsNullOrEmpty(c) || symbols.Contains(c) || numbers.Contains(c))
            {
                lex = expectLex;
                return true;
            }

            //build error message
            var s = ReadLex();
            throw new SyntaxException($"expect : '{expectLex}', actual : '{s}'");
        }
        return false;
    }

    public void Dispose()
    {
        //((IDisposable)Reader).Dispose();
    }
}