using Carbunql.Core.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

public class LexReader : CharReader
{
    public LexReader(string text) : base(text)
    {
    }

    public IEnumerable<char> ForceBreakSymbols { get; set; } = ".,();".ToArray();

    public IEnumerable<char> BitwiseOperatorSymbols { get; set; } = "&|^#~".ToArray();

    public IEnumerable<char> ArithmeticOperatorSymbols { get; set; } = "+-*/".ToArray();

    public IEnumerable<char> ComparisonOperatorSymbols { get; set; } = "<>!=".ToArray();

    public IEnumerable<char> PrefixSymbols { get; set; } = "?:@".ToArray();

    public IEnumerable<char> SingleSymbols => ForceBreakSymbols.Union(BitwiseOperatorSymbols);

    public IEnumerable<char> MultipleSymbols => ArithmeticOperatorSymbols.Union(ComparisonOperatorSymbols);

    public IEnumerable<char> AllSymbols => SingleSymbols.Union(MultipleSymbols).Union(PrefixSymbols).Union(SpaceChars);

    public string ReadLex(bool skipSpace = true)
    {
        if (skipSpace) SkipSpace();

        using var sb = ZString.CreateStringBuilder();

        var fc = ReadChars().FirstOrDefault();
        if (fc == '\0') return string.Empty;

        sb.Append(fc);

        // ex. 'text'
        if (fc == '\'')
        {
            sb.Append(ReadUntilSingleQuote());
            return sb.ToString();
        }

        // ex. | or ||
        if (fc == '|')
        {
            if (PeekAreEqual('|')) sb.Append(ReadChar());
            return sb.ToString();
        }

        // ex. - or --
        if (fc == '-')
        {
            if (PeekAreEqual('-')) sb.Append(ReadChar());
            return sb.ToString();
        }

        // ex. / or /*
        if (fc == '/')
        {
            if (PeekAreEqual('*')) sb.Append(ReadChar());
            return sb.ToString();
        }

        // ex. * or */
        if (fc == '*')
        {
            if (PeekAreEqual('/')) sb.Append(ReadChar());
            return sb.ToString();
        }

        // ex. . or , or (
        if (SingleSymbols.Contains(fc)) return sb.ToString();

        // ex. + or !=
        if (MultipleSymbols.Contains(fc))
        {
            foreach (var item in ReadWhile((x) => x != '/' && x != '*' && MultipleSymbols.Contains(x)))
            {
                sb.Append(item);
            }
            return sb.ToString();
        }

        // ex. 123.45
        if (fc.IsInteger())
        {
            foreach (var item in ReadWhile((x) => "0123456789.".ToArray().Contains(x)))
            {
                sb.Append(item);
            }
            return sb.ToString();
        }

        var whileFn = (char c) =>
        {
            if (AllSymbols.Contains(c)) return false;
            return true;
        };

        foreach (var item in ReadWhile(whileFn))
        {
            sb.Append(item);
        }
        return sb.ToString();
    }

    public IEnumerable<string> ReadLexs(bool skipSpace = true)
    {
        var w = ReadLex(skipSpace);
        while (!string.IsNullOrEmpty(w))
        {
            yield return w;
            w = ReadLex(skipSpace);
        }
    }
}