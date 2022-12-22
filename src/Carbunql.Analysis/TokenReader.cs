using Carbunql.Core.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

public class TokenReader : LexReader
{
    public TokenReader(string text) : base(text)
    {
    }

    public static string?[] BreakTokens = new string?[]
    {
        null,
        string.Empty,
        ",",
        "with",
        "values",
        "select",
        "from",
        "where",
        "group",
        "having",
        "order",
        "union",
        "minus",
        "except",
        "intersect"
    };

    private string? TokenCache { get; set; } = string.Empty;

    public string? PeekRawToken(bool skipComment = true)
    {
        if (string.IsNullOrEmpty(TokenCache))
        {
            TokenCache = ReadRawToken(skipSpace: true);
        }

        if (!skipComment || string.IsNullOrEmpty(TokenCache)) return TokenCache;

        var tokens = new string[] { "--", "/*" };
        while (TokenCache.AreContains(tokens))
        {
            var t = ReadToken(skipComment: false);
            if (t == "--")
            {
                ReadUntilLineEnd();
            }
            else
            {
                ReadUntilCloseBlockComment();
            }
            TokenCache = ReadRawToken(skipSpace: true);
        }
        return TokenCache;
    }

    public string? TryReadToken(string expectRawToken)
    {
        var s = PeekRawToken();
        if (!s.AreEqual(expectRawToken)) return null;
        return ReadToken();
    }

    public string ReadToken(string expectRawToken)
    {
        var s = PeekRawToken();
        if (string.IsNullOrEmpty(s)) throw new SyntaxException($"expect '{expectRawToken}', actual is empty.");
        if (!s.AreEqual(expectRawToken)) throw new SyntaxException($"expect '{expectRawToken}', actual '{s}'.");
        return ReadToken();
    }

    public string ReadToken(string[] expectRawTokens)
    {
        var s = PeekRawToken();
        if (string.IsNullOrEmpty(s)) throw new SyntaxException($"token is empty.");
        if (!s.AreContains(expectRawTokens)) throw new SyntaxException($"near '{s}'.");
        return ReadToken();
    }

    public string ReadToken(bool skipComment = true)
    {
        string? token = ReadRawToken();
        if (string.IsNullOrEmpty(token)) return string.Empty;

        // Explore possible two-word tokens
        if (token.AreEqual("is"))
        {
            if (PeekRawToken().AreEqual("not"))
            {
                return token + " " + ReadToken("not");
            }
            return token;
        }

        if (token.AreContains(new string[] { "inner", "cross" }))
        {
            var outer = TryReadToken("outer");
            if (!string.IsNullOrEmpty(outer)) token += " " + outer;
            var t = ReadToken("join");
            return token + " " + t;
        }

        if (token.AreContains(new string[] { "group", "partition", "order" }))
        {
            var t = ReadToken("by");
            return token + " " + t;
        }

        if (token.AreContains(new string[] { "left", "right" }))
        {
            if (PeekRawToken().AreEqual("(")) return token;
            var outer = TryReadToken("outer");
            if (!string.IsNullOrEmpty(outer)) token += " " + outer;
            var t = ReadToken("join");
            return token + " " + t;
        }

        if (token.AreEqual("nulls"))
        {
            var t = ReadToken(new string[] { "first", "last" });
            return token + " " + t;
        }

        if (token.AreEqual("union"))
        {
            if (PeekRawToken().AreEqual("all"))
            {
                return token + " " + ReadToken("all");
            }
            return token;
        }

        if (token.AreEqual("not"))
        {
            if (PeekRawToken().AreEqual("materialized"))
            {
                return token + " " + ReadToken("materialized");
            }
            return token;
        }

        if (token.AreEqual(":"))
        {
            //ex ::text
            return token + ReadToken();
        }

        if (!skipComment) return token;

        if (token == "--")
        {
            ReadUntilLineEnd();
            return ReadToken(skipComment);
        }

        if (token == "/*")
        {
            ReadUntilCloseBlockComment();
            return ReadToken(skipComment);
        }
        return token;
    }

    private string? ReadRawToken(bool skipSpace = true)
    {
        if (!string.IsNullOrEmpty(TokenCache))
        {
            var s = TokenCache;
            TokenCache = string.Empty;
            return s;
        }
        return ReadLexs(skipSpace).FirstOrDefault();
    }

    private IEnumerable<string> ReadRawTokens(bool skipSpace = true)
    {
        var token = ReadRawToken(skipSpace: skipSpace);
        while (!string.IsNullOrEmpty(token))
        {
            yield return token;
            token = ReadRawToken(skipSpace: skipSpace);
        }
    }

    internal (string first, string inner) ReadUntilCloseBracket()
    {
        SkipSpace();
        using var sb = ZString.CreateStringBuilder();
        var fs = string.Empty;

        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;
            if (string.IsNullOrEmpty(fs)) fs = word;

            if (word.AreEqual(")"))
            {
                return (fs, sb.ToString());
            }

            if (word.AreEqual("("))
            {
                var (_, inner) = ReadUntilCloseBracket();
                sb.Append("(" + inner + ")");
            }
            else
            {
                sb.Append(word);
            }
        }

        throw new SyntaxException("bracket is not closed");
    }

    private string ReadUntilCloseBlockComment()
    {
        using var inner = ZString.CreateStringBuilder();

        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;

            inner.Append(word);
            if (word.AreEqual("*/"))
            {
                return inner.ToString();
            }
            if (word.AreEqual("/*"))
            {
                inner.Append(ReadUntilCloseBlockComment());
            }
        }

        throw new SyntaxException("block comment is not closed");
    }

    internal string ReadUntilCaseExpressionEnd()
    {
        using var inner = ZString.CreateStringBuilder();

        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;

            inner.Append(word);
            if (word.TrimStart().AreEqual("end"))
            {
                return inner.ToString();
            }
            if (word.TrimStart().AreEqual("case"))
            {
                inner.Append(ReadUntilCaseExpressionEnd());
            }
        }

        throw new SyntaxException("case expression is not end");
    }

    internal string ReadUntilToken(string breaktoken)
    {
        return ReadUntilToken(x => x.AreEqual(breaktoken));
    }

    internal string ReadUntilToken(Func<string, bool> fn)
    {
        using var inner = ZString.CreateStringBuilder();

        SkipSpace();
        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;
            if (fn(word.TrimStart()))
            {
                return inner.ToString();
            }
            inner.Append(word);
        }

        throw new SyntaxException($"breaktoken token is not found");
    }
}