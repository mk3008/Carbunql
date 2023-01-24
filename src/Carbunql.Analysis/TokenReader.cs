using Carbunql.Extensions;
using Cysharp.Text;
using System.Reflection.PortableExecutable;

namespace Carbunql.Analysis;

public class TokenReader : LexReader, ITokenReader
{
	public TokenReader(string text) : base(text)
	{
		BreakTokens = (new List<string>() { ";" }).AsReadOnly();
	}

	public IReadOnlyList<string> BreakTokens { get; init; }

	private string? TokenCache { get; set; } = string.Empty;

	public int CommentLevel { get; private set; } = 0;

	private bool IsTerminated { get; set; } = false;

	public string TerminatedToken { get; private set; } = string.Empty;

	private string? ReadTokenMain()
	{
		//Token reading processing and termination judgment are performed

		if (!string.IsNullOrEmpty(TokenCache))
		{
			var s = TokenCache;
			TokenCache = null;
			return s;
		}

		if (IsTerminated) return null;

		var tmp = ReadLexs(skipSpace: true).FirstOrDefault();

		//skip comment block
		var commentTokens = new string[] { "--", "/*" };
		while (tmp.AreContains(commentTokens))
		{
			var t = ReadToken();
			if (t == "--")
			{
				//line comment
				CommentLevel++;
				ReadUntilLineEnd();
				CommentLevel--;
			}
			else
			{
				//block comment
				CommentLevel++;
				ReadUntilCloseBlockComment();
				CommentLevel--;
			}
			tmp = ReadLexs(skipSpace: true).FirstOrDefault();
		}

		// termination check
		if (string.IsNullOrEmpty(tmp))
		{
			IsTerminated = true;
			TerminatedToken = string.Empty;
		}
		else if (CommentLevel == 0 && tmp.AreContains(BreakTokens))
		{
			IsTerminated = true;
			TerminatedToken = tmp;
		}

		return tmp;
	}

	public string? PeekRawToken()
	{
		//Cache ReadRawToken to implement peek functionality
		if (!string.IsNullOrEmpty(TokenCache)) return TokenCache;

		TokenCache = ReadTokenMain();
		return TokenCache;
	}

	public string ReadToken()
	{
		var token = ReadTokenMain();
		if (token == null) return string.Empty;

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

		if (token == "--")
		{
			ReadUntilLineEnd();
			return ReadToken();
		}

		if (token == "/*")
		{
			this.ReadUntilCloseBlockComment();
			return ReadToken();
		}

		return token;
	}

	public string ReadUntilToken(string breaktoken)
	{
		return ReadUntilToken(x => x.AreEqual(breaktoken));
	}

	public string ReadUntilToken(Func<string, bool> fn)
	{
		using var inner = ZString.CreateStringBuilder();

		SkipSpace();
		foreach (var word in ReadLexs(skipSpace: false))
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

	public string ReadUntilCloseBlockComment()
	{
		using var inner = ZString.CreateStringBuilder();

		foreach (var word in ReadLexs(skipSpace: false))
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
}