using Carbunql.Extensions;
using Cysharp.Text;

namespace Carbunql.Analysis;

public class TokenReader : LexReader, ITokenReader
{
	public TokenReader(string text) : base(text)
	{
		BreakTokens = new() { ";" };
	}

	public List<string> BreakTokens { get; init; }

	private string? TokenCache { get; set; } = string.Empty;

	private int CommentLevel { get; set; } = 0;

	private bool IsTerminate { get; set; } = false;

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
				CommentLevel++;
				ReadUntilLineEnd();
				CommentLevel--;
			}
			else
			{
				CommentLevel++;
				this.ReadUntilCloseBlockComment();
				CommentLevel--;
			}
			TokenCache = ReadRawToken(skipSpace: true);
		}
		return TokenCache;
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
				return token + " " + this.ReadToken("not");
			}
			return token;
		}

		if (token.AreContains(new string[] { "inner", "cross" }))
		{
			var outer = this.TryReadToken("outer");
			if (!string.IsNullOrEmpty(outer)) token += " " + outer;
			var t = this.ReadToken("join");
			return token + " " + t;
		}

		if (token.AreContains(new string[] { "group", "partition", "order" }))
		{
			var t = this.ReadToken("by");
			return token + " " + t;
		}

		if (token.AreContains(new string[] { "left", "right" }))
		{
			if (PeekRawToken().AreEqual("(")) return token;
			var outer = this.TryReadToken("outer");
			if (!string.IsNullOrEmpty(outer)) token += " " + outer;
			var t = this.ReadToken("join");
			return token + " " + t;
		}

		if (token.AreEqual("nulls"))
		{
			var t = this.ReadToken(new string[] { "first", "last" });
			return token + " " + t;
		}

		if (token.AreEqual("union"))
		{
			if (PeekRawToken().AreEqual("all"))
			{
				return token + " " + this.ReadToken("all");
			}
			return token;
		}

		if (token.AreEqual("not"))
		{
			if (PeekRawToken().AreEqual("materialized"))
			{
				return token + " " + this.ReadToken("materialized");
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
			this.ReadUntilCloseBlockComment();
			return ReadToken(skipComment);
		}
		return token;
	}

	public string? ReadRawToken(bool skipSpace = true)
	{
		if (IsTerminate) return null;

		if (!string.IsNullOrEmpty(TokenCache))
		{
			var s = TokenCache;
			TokenCache = string.Empty;
			return s;
		}
		var t = ReadLexs(skipSpace).FirstOrDefault();

		// terminate
		if (CommentLevel == 0 && t.AreContains(BreakTokens))
		{
			IsTerminate = true;
			return null;
		}

		return t;
	}

	public (string first, string inner) ReadUntilCloseBracket()
	{
		SkipSpace();
		using var sb = ZString.CreateStringBuilder();
		var fs = string.Empty;

		foreach (var word in this.ReadRawTokens(skipSpace: false))
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
}